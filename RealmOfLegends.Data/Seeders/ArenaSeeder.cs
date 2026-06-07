using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities.Arena;
using RealmOfLegends.Data.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RealmOfLegends.Data.Seeders
{
    public static class ArenaSeeder
    {
        public static async Task SeedEnemiesAsync(GameDbContext context)
        {
            if (await context.Enemies.AnyAsync())
            {
                return; // Already seeded
            }

            var enemies = new[]
            {
                // Level 1: Goblin Scout
                new Enemy
                {
                    Name = "Goblin Scout",
                    Level = 1,
                    IsBoss = false,
                    BaseHP = 120,
                    BaseATK = 15,
                    BaseDEF = 5,
                    EvasionChance = 10,
                    SpriteSheetPath = "/images/arena/goblin-scout.png",
                    FrameWidth = 64,
                    FrameHeight = 64,
                    AnimationFPS = 12,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 6,
                    Attack2Frames = 6,
                    HeavyFrames = 8,
                    SpecialFrames = 10,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 8,
                    RenderScale = 1.0f,
                    GroundShadowWidth = 80,
                    SpecialAbilityName = "Quick Jab",
                    SpecialAbilityDamageMultiplier = 1.2f,
                    DropGoldMin = 20,
                    DropGoldMax = 50,
                    DropXP = 80,
                    DropItemChance = 10,
                    LightingTint = "#2d5a1b",
                    ParticleTheme = "nature"
                },
                
                // Level 2: Skeleton Warrior
                new Enemy
                {
                    Name = "Skeleton Warrior",
                    Level = 2,
                    IsBoss = false,
                    BaseHP = 200,
                    BaseATK = 25,
                    BaseDEF = 12,
                    EvasionChance = 8,
                    SpriteSheetPath = "/images/arena/skeleton-warrior.png",
                    FrameWidth = 64,
                    FrameHeight = 64,
                    AnimationFPS = 12,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 6,
                    Attack2Frames = 6,
                    HeavyFrames = 8,
                    SpecialFrames = 10,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 8,
                    RenderScale = 1.0f,
                    GroundShadowWidth = 80,
                    SpecialAbilityName = "Bone Shield",
                    SpecialAbilityDamageMultiplier = 1.3f,
                    DropGoldMin = 40,
                    DropGoldMax = 80,
                    DropXP = 150,
                    DropItemChance = 12,
                    LightingTint = "#1a1a3d",
                    ParticleTheme = "arcane"
                },
                
                // Level 3: Forest Wolf
                new Enemy
                {
                    Name = "Forest Wolf",
                    Level = 3,
                    IsBoss = false,
                    BaseHP = 170,
                    BaseATK = 30,
                    BaseDEF = 8,
                    EvasionChance = 18,
                    SpriteSheetPath = "/images/arena/forest-wolf.png",
                    FrameWidth = 64,
                    FrameHeight = 64,
                    AnimationFPS = 12,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 6,
                    Attack2Frames = 6,
                    HeavyFrames = 8,
                    SpecialFrames = 10,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 8,
                    RenderScale = 1.1f,
                    GroundShadowWidth = 90,
                    SpecialAbilityName = "Pack Bite",
                    SpecialAbilityDamageMultiplier = 1.3f,
                    DropGoldMin = 60,
                    DropGoldMax = 100,
                    DropXP = 200,
                    DropItemChance = 15,
                    LightingTint = "#2a0d3d",
                    ParticleTheme = "nature"
                },
                
                // Level 4: Dark Elf Archer
                new Enemy
                {
                    Name = "Dark Elf Archer",
                    Level = 4,
                    IsBoss = false,
                    BaseHP = 280,
                    BaseATK = 40,
                    BaseDEF = 15,
                    EvasionChance = 22,
                    SpriteSheetPath = "/images/arena/dark-elf-archer.png",
                    FrameWidth = 64,
                    FrameHeight = 64,
                    AnimationFPS = 12,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 6,
                    Attack2Frames = 6,
                    HeavyFrames = 8,
                    SpecialFrames = 10,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 8,
                    RenderScale = 1.0f,
                    GroundShadowWidth = 80,
                    SpecialAbilityName = "Poison Arrow",
                    SpecialAbilityDamageMultiplier = 1.4f,
                    DropGoldMin = 80,
                    DropGoldMax = 130,
                    DropXP = 280,
                    DropItemChance = 18,
                    LightingTint = "#0d2a2a",
                    ParticleTheme = "arcane"
                },
                
                // Level 5: Golem of Ruin (BOSS)
                new Enemy
                {
                    Name = "Golem of Ruin",
                    Level = 5,
                    IsBoss = true,
                    BaseHP = 1200,
                    BaseATK = 70,
                    BaseDEF = 35,
                    EvasionChance = 5,
                    SpriteSheetPath = "/images/arena/golem-of-ruin.png",
                    FrameWidth = 128,
                    FrameHeight = 128,
                    AnimationFPS = 10,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 8,
                    Attack2Frames = 8,
                    HeavyFrames = 10,
                    SpecialFrames = 12,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 10,
                    RenderScale = 2.2f,
                    GroundShadowWidth = 150,
                    SpecialAbilityName = "Earthquake Slam",
                    SpecialAbilityDamageMultiplier = 1.8f,
                    DropGoldMin = 300,
                    DropGoldMax = 500,
                    DropXP = 800,
                    DropItemChance = 40,
                    LightingTint = "#4a1000",
                    ParticleTheme = "fire"
                },
                
                // Level 6: Ice Banshee
                new Enemy
                {
                    Name = "Ice Banshee",
                    Level = 6,
                    IsBoss = false,
                    BaseHP = 350,
                    BaseATK = 55,
                    BaseDEF = 20,
                    EvasionChance = 25,
                    SpriteSheetPath = "/images/arena/ice-banshee.png",
                    FrameWidth = 64,
                    FrameHeight = 64,
                    AnimationFPS = 12,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 6,
                    Attack2Frames = 6,
                    HeavyFrames = 8,
                    SpecialFrames = 10,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 8,
                    RenderScale = 1.1f,
                    GroundShadowWidth = 80,
                    SpecialAbilityName = "Wail Stun",
                    SpecialAbilityDamageMultiplier = 1.5f,
                    DropGoldMin = 120,
                    DropGoldMax = 180,
                    DropXP = 380,
                    DropItemChance = 20,
                    LightingTint = "#001a4a",
                    ParticleTheme = "ice"
                },
                
                // Level 7: Stone Golem Junior
                new Enemy
                {
                    Name = "Stone Golem Junior",
                    Level = 7,
                    IsBoss = false,
                    BaseHP = 500,
                    BaseATK = 65,
                    BaseDEF = 30,
                    EvasionChance = 10,
                    SpriteSheetPath = "/images/arena/stone-golem-junior.png",
                    FrameWidth = 80,
                    FrameHeight = 80,
                    AnimationFPS = 10,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 7,
                    Attack2Frames = 7,
                    HeavyFrames = 9,
                    SpecialFrames = 11,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 9,
                    RenderScale = 1.3f,
                    GroundShadowWidth = 100,
                    SpecialAbilityName = "Rock Throw",
                    SpecialAbilityDamageMultiplier = 1.4f,
                    DropGoldMin = 150,
                    DropGoldMax = 220,
                    DropXP = 450,
                    DropItemChance = 22,
                    LightingTint = "#1a1208",
                    ParticleTheme = "earth"
                },
                
                // Level 8: Shadow Assassin
                new Enemy
                {
                    Name = "Shadow Assassin",
                    Level = 8,
                    IsBoss = false,
                    BaseHP = 420,
                    BaseATK = 80,
                    BaseDEF = 25,
                    EvasionChance = 35,
                    SpriteSheetPath = "/images/arena/shadow-assassin.png",
                    FrameWidth = 64,
                    FrameHeight = 64,
                    AnimationFPS = 14,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 6,
                    Attack2Frames = 6,
                    HeavyFrames = 8,
                    SpecialFrames = 10,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 8,
                    RenderScale = 1.0f,
                    GroundShadowWidth = 75,
                    SpecialAbilityName = "Backstab",
                    SpecialAbilityDamageMultiplier = 1.6f,
                    DropGoldMin = 180,
                    DropGoldMax = 260,
                    DropXP = 530,
                    DropItemChance = 25,
                    LightingTint = "#0d002a",
                    ParticleTheme = "shadow"
                },
                
                // Level 9: Fire Drake
                new Enemy
                {
                    Name = "Fire Drake",
                    Level = 9,
                    IsBoss = false,
                    BaseHP = 700,
                    BaseATK = 95,
                    BaseDEF = 40,
                    EvasionChance = 15,
                    SpriteSheetPath = "/images/arena/fire-drake.png",
                    FrameWidth = 96,
                    FrameHeight = 96,
                    AnimationFPS = 12,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 7,
                    Attack2Frames = 7,
                    HeavyFrames = 9,
                    SpecialFrames = 12,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 9,
                    RenderScale = 1.5f,
                    GroundShadowWidth = 120,
                    SpecialAbilityName = "Fire Breath",
                    SpecialAbilityDamageMultiplier = 1.7f,
                    DropGoldMin = 220,
                    DropGoldMax = 320,
                    DropXP = 680,
                    DropItemChance = 30,
                    LightingTint = "#4a0800",
                    ParticleTheme = "fire"
                },
                
                // Level 10: Lich King (BOSS)
                new Enemy
                {
                    Name = "Lich King",
                    Level = 10,
                    IsBoss = true,
                    BaseHP = 3000,
                    BaseATK = 130,
                    BaseDEF = 55,
                    EvasionChance = 8,
                    SpriteSheetPath = "/images/arena/lich-king.png",
                    FrameWidth = 128,
                    FrameHeight = 128,
                    AnimationFPS = 10,
                    IdleFrames = 4,
                    WalkFrames = 6,
                    Attack1Frames = 8,
                    Attack2Frames = 8,
                    HeavyFrames = 10,
                    SpecialFrames = 15,
                    HitFrames = 3,
                    StaggerFrames = 4,
                    KnockbackFrames = 5,
                    DeathFrames = 12,
                    RenderScale = 2.5f,
                    GroundShadowWidth = 180,
                    SpecialAbilityName = "Death Coil",
                    SpecialAbilityDamageMultiplier = 2.0f,
                    DropGoldMin = 800,
                    DropGoldMax = 1200,
                    DropXP = 2000,
                    DropItemChance = 60,
                    LightingTint = "#000000",
                    ParticleTheme = "shadow"
                }
            };

            await context.Enemies.AddRangeAsync(enemies);
            await context.SaveChangesAsync();
        }
    }
}
