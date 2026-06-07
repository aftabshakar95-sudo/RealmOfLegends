using System;

namespace RealmOfLegends.Core.Entities.Arena
{
    public class Enemy
    {
        public int EnemyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; } // 1-10
        public bool IsBoss { get; set; }
        
        // Base Stats
        public int BaseHP { get; set; }
        public int BaseATK { get; set; }
        public int BaseDEF { get; set; }
        public int EvasionChance { get; set; } // 0-100 percentage
        
        // Sprite Configuration
        public string SpriteSheetPath { get; set; } = string.Empty;
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int AnimationFPS { get; set; } = 12;
        
        // Animation Frame Counts
        public int IdleFrames { get; set; } = 4;
        public int WalkFrames { get; set; } = 6;
        public int Attack1Frames { get; set; } = 6;
        public int Attack2Frames { get; set; } = 6;
        public int HeavyFrames { get; set; } = 8;
        public int SpecialFrames { get; set; } = 10;
        public int HitFrames { get; set; } = 3;
        public int StaggerFrames { get; set; } = 4;
        public int KnockbackFrames { get; set; } = 5;
        public int DeathFrames { get; set; } = 8;
        
        // Visual Settings
        public float RenderScale { get; set; } = 1.0f; // Bosses use 2.0+
        public int GroundShadowWidth { get; set; } = 80;
        
        // Special Ability
        public string SpecialAbilityName { get; set; } = string.Empty;
        public float SpecialAbilityDamageMultiplier { get; set; } = 1.5f;
        
        // Rewards
        public int DropGoldMin { get; set; }
        public int DropGoldMax { get; set; }
        public int DropXP { get; set; }
        public int DropItemChance { get; set; } // 0-100 percentage
        
        // Visual Effects
        public string LightingTint { get; set; } = "#FFFFFF"; // Hex color for scene ambient
        public string ParticleTheme { get; set; } = "fire"; // fire/ice/shadow/nature/arcane/earth
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
