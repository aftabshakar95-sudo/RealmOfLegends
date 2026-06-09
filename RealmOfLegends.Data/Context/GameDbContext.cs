using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Core.Entities.Arena;

namespace RealmOfLegends.Data.Context
{
    public class GameDbContext : IdentityDbContext<ApplicationUser>
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<PlayerQuest> PlayerQuests { get; set; }
        public DbSet<ArenaProgress> ArenaProgresses { get; set; }
        public DbSet<FightLog> FightLogs { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<PlayerAchievement> PlayerAchievements { get; set; }
        public DbSet<News> News { get; set; }
        
        // New Arena System
        public DbSet<Enemy> Enemies { get; set; }
        public DbSet<FightSession> FightSessions { get; set; }
        public DbSet<ArenaProgressNew> ArenaProgressNews { get; set; }
        public DbSet<FightLogNew> FightLogNews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Player Configuration
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.PlayerId);
                entity.HasIndex(e => e.UserId).IsUnique();
                entity.Property(e => e.CharacterName).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Class).HasMaxLength(20).IsRequired();
                entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
            });

            // Item Configuration
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.ItemId);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Rarity).HasMaxLength(20).IsRequired();
                entity.Property(e => e.StatJson).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.ImagePath).HasMaxLength(200);
                entity.Property(e => e.SlotType).HasMaxLength(20);
            });

            // Inventory Configuration
            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasKey(e => e.InventoryId);
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.Inventories)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Item)
                    .WithMany(i => i.Inventories)
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.SlotType).HasMaxLength(20);
            });

            // PurchaseHistory Configuration
            modelBuilder.Entity<PurchaseHistory>(entity =>
            {
                entity.HasKey(e => e.PurchaseId);
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.PurchaseHistories)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Item)
                    .WithMany(i => i.PurchaseHistories)
                    .HasForeignKey(e => e.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Quest Configuration
            modelBuilder.Entity<Quest>(entity =>
            {
                entity.HasKey(e => e.QuestId);
                entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.QuestData).HasColumnType("nvarchar(max)");
                entity.HasOne(e => e.RewardItem)
                    .WithMany()
                    .HasForeignKey(e => e.RewardItemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PlayerQuest Configuration
            modelBuilder.Entity<PlayerQuest>(entity =>
            {
                entity.HasKey(e => e.PlayerQuestId);
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.PlayerQuests)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Quest)
                    .WithMany(q => q.PlayerQuests)
                    .HasForeignKey(e => e.QuestId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Status).HasMaxLength(20);
            });

            // ArenaProgress Configuration
            modelBuilder.Entity<ArenaProgress>(entity =>
            {
                entity.HasKey(e => e.ArenaProgressId);
                entity.HasOne(e => e.Player)
                    .WithOne(p => p.ArenaProgress)
                    .HasForeignKey<ArenaProgress>(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // FightLog Configuration
            modelBuilder.Entity<FightLog>(entity =>
            {
                entity.HasKey(e => e.FightLogId);
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.FightLogs)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.DroppedItem)
                    .WithMany()
                    .HasForeignKey(e => e.DroppedItemId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Outcome).HasMaxLength(20);
            });

            // Achievement Configuration
            modelBuilder.Entity<Achievement>(entity =>
            {
                entity.HasKey(e => e.AchievementId);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Title).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IconPath).HasMaxLength(200);
            });

            // PlayerAchievement Configuration
            modelBuilder.Entity<PlayerAchievement>(entity =>
            {
                entity.HasKey(e => e.PlayerAchievementId);
                entity.HasOne(e => e.Player)
                    .WithMany(p => p.PlayerAchievements)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Achievement)
                    .WithMany(a => a.PlayerAchievements)
                    .HasForeignKey(e => e.AchievementId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.PlayerId, e.AchievementId }).IsUnique();
            });

            // News Configuration
            modelBuilder.Entity<News>(entity =>
            {
                entity.HasKey(e => e.NewsId);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Body).HasColumnType("nvarchar(max)");
            });
            
            // Enemy Configuration
            modelBuilder.Entity<Enemy>(entity =>
            {
                entity.HasKey(e => e.EnemyId);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.SpriteSheetPath).HasMaxLength(200);
                entity.Property(e => e.SpecialAbilityName).HasMaxLength(100);
                entity.Property(e => e.LightingTint).HasMaxLength(7);
                entity.Property(e => e.ParticleTheme).HasMaxLength(20);
            });
            
            // FightSession Configuration
            modelBuilder.Entity<FightSession>(entity =>
            {
                entity.HasKey(e => e.FightSessionId);
                entity.Property(e => e.FightSessionId).HasMaxLength(50);
                entity.HasOne(e => e.Player)
                    .WithMany()
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Enemy)
                    .WithMany()
                    .HasForeignKey(e => e.EnemyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            
            // ArenaProgressNew Configuration
            modelBuilder.Entity<ArenaProgressNew>(entity =>
            {
                entity.HasKey(e => e.ArenaProgressNewId);
                entity.HasOne(e => e.Player)
                    .WithMany()
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.PlayerId).IsUnique();
            });
            
            // FightLogNew Configuration
            modelBuilder.Entity<FightLogNew>(entity =>
            {
                entity.HasKey(e => e.FightLogNewId);
                entity.HasOne(e => e.Player)
                    .WithMany()
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Enemy)
                    .WithMany()
                    .HasForeignKey(e => e.EnemyId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.DroppedItem)
                    .WithMany()
                    .HasForeignKey(e => e.DroppedItemId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Outcome).HasMaxLength(20);
            });
        }
    }
}
