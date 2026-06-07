using RealmOfLegends.Core.Entities.Arena;

namespace RealmOfLegends.Core.Services.Arena
{
    public class FightSessionDTO
    {
        public string SessionId { get; set; } = string.Empty;
        public int ArenaLevel { get; set; }
        public PlayerStatsDTO Player { get; set; } = new();
        public EnemyDataDTO Enemy { get; set; } = new();
    }

    public class PlayerStatsDTO
    {
        public int PlayerId { get; set; }
        public string CharacterName { get; set; } = string.Empty;
        public string Class { get; set; } = string.Empty;
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public int CurrentMP { get; set; }
        public int MaxMP { get; set; }
        public int AttackPower { get; set; }
        public int DefenseRating { get; set; }
        public int Level { get; set; }
        public double CritChance { get; set; }
    }

    public class EnemyDataDTO
    {
        public int EnemyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public bool IsBoss { get; set; }
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public int BaseATK { get; set; }
        public int BaseDEF { get; set; }
        public int EvasionChance { get; set; }
        
        // Sprite Configuration
        public string SpriteSheetPath { get; set; } = string.Empty;
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int AnimationFPS { get; set; }
        public int IdleFrames { get; set; }
        public int WalkFrames { get; set; }
        public int Attack1Frames { get; set; }
        public int Attack2Frames { get; set; }
        public int HeavyFrames { get; set; }
        public int SpecialFrames { get; set; }
        public int HitFrames { get; set; }
        public int StaggerFrames { get; set; }
        public int KnockbackFrames { get; set; }
        public int DeathFrames { get; set; }
        public float RenderScale { get; set; }
        public int GroundShadowWidth { get; set; }
        
        public string SpecialAbilityName { get; set; } = string.Empty;
        public float SpecialAbilityDamageMultiplier { get; set; }
        public string LightingTint { get; set; } = string.Empty;
        public string ParticleTheme { get; set; } = string.Empty;
    }

    public class TurnResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int PlayerDamage { get; set; }
        public bool IsCritical { get; set; }
        public bool IsMiss { get; set; }
        public int EnemyHPRemaining { get; set; }
        public int EnemyMaxHP { get; set; }
        public double EnemyHPPercent { get; set; }
        public bool IsVictory { get; set; }
        public VictoryRewardDTO? VictoryReward { get; set; }
        public string AnimationType { get; set; } = "basicAttack"; // basicAttack, heavyStrike, special
        public float KnockbackForce { get; set; } = 1.0f;
        public string ParticleType { get; set; } = "impact";
    }

    public class EnemyTurnResultDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int EnemyDamage { get; set; }
        public string AbilityUsed { get; set; } = string.Empty;
        public string AbilityName { get; set; } = string.Empty;
        public int PlayerHPRemaining { get; set; }
        public int PlayerMaxHP { get; set; }
        public double PlayerHPPercent { get; set; }
        public bool IsDefeat { get; set; }
        public string AnimationType { get; set; } = "attack1";
        public float KnockbackForce { get; set; } = 1.0f;
        public float ScreenShakeIntensity { get; set; } = 0.0f; // 0=none, 0.5=special, 1.0=boss special
        public string ParticleType { get; set; } = "impact";
        public string LightFlashColor { get; set; } = "#FFFFFF";
        public int BleedDamage { get; set; } = 0; // For Rogue bleed effect
    }

    public class VictoryRewardDTO
    {
        public int GoldEarned { get; set; }
        public int XPEarned { get; set; }
        public string? DroppedItemName { get; set; }
        public int? DroppedItemId { get; set; }
        public bool LeveledUp { get; set; }
        public int NewLevel { get; set; }
        public int NewArenaLevel { get; set; }
    }

    public class ArenaLevelViewModel
    {
        public int Level { get; set; }
        public string EnemyName { get; set; } = string.Empty;
        public string EnemyType { get; set; } = string.Empty;
        public bool IsBoss { get; set; }
        public string Status { get; set; } = "locked"; // locked, available, cleared
        public string BackgroundPreviewColor { get; set; } = "#000000";
        public string RewardPreview { get; set; } = string.Empty;
    }

    // Hub DTOs for the ArenaCanvas/Index level select page
    public class ArenaHubDto
    {
        public List<ArenaEnemyViewModel> Enemies { get; set; } = new();
        public int VictoriesCount { get; set; }
        public int TotalEnemies { get; set; }
        public int TotalKills { get; set; }
        public int HighestLevelCompleted { get; set; }
        public List<int> Victories { get; set; } = new(); // EnemyIds the player has defeated
    }

    public class ArenaEnemyViewModel
    {
        public int Id { get; set; }
        public int LevelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsBoss { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
