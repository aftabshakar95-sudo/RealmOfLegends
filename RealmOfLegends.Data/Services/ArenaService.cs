using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Core.Entities.Arena;
using RealmOfLegends.Core.Services.Arena;
using RealmOfLegends.Data.Context;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RealmOfLegends.Data.Services
{
    public class ArenaService : IArenaService
    {
        private readonly GameDbContext _context;
        private readonly ILogger<ArenaService> _logger;
        private readonly Random _random = new();

        public ArenaService(GameDbContext context, ILogger<ArenaService> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ArenaHubDto> GetArenaHubDataAsync(int playerId)
        {
            var enemies = await _context.Enemies.OrderBy(e => e.Level).ToListAsync();
            var progress = await _context.ArenaProgressNews
                .FirstOrDefaultAsync(ap => ap.PlayerId == playerId);

            var victories = await _context.FightLogNews
                .Where(fl => fl.PlayerId == playerId && fl.Outcome == "Victory")
                .Select(fl => fl.EnemyId)
                .Distinct()
                .ToListAsync();

            int highestCleared = progress?.HighestLevelCleared ?? 0;
            int totalWins = progress?.TotalWins ?? 0;

            var descriptions = new Dictionary<int, string>
            {
                { 1, "A nimble goblin scout lurking in the forest. Quick but weak." },
                { 2, "An undead warrior risen from ancient battlefields. Relentless in combat." },
                { 3, "A fierce wolf prowling the dark woods. Fast and deadly." },
                { 4, "A dark elf skilled in archery and poison. Evasive and cunning." },
                { 5, "A massive golem forged from volcanic rock. Devastating power!" },
                { 6, "A spectral banshee from the frozen wastes. Her wail freezes the soul." },
                { 7, "A young golem apprentice learning the ways of stone." },
                { 8, "A master of stealth striking from the shadows. Lethal precision." },
                { 9, "A fire-breathing drake from the molten caverns. Pure fury." },
                { 10, "The undead Lich King. Master of death magic. The ultimate challenge!" }
            };

            return new ArenaHubDto
            {
                Enemies = enemies.Select(e => new ArenaEnemyViewModel
                {
                    Id = e.EnemyId,
                    LevelId = e.Level,
                    Name = e.Name,
                    IsBoss = e.IsBoss,
                    Health = e.BaseHP,
                    Attack = e.BaseATK,
                    Defense = e.BaseDEF,
                    Description = descriptions.GetValueOrDefault(e.Level, "A dangerous foe awaits...")
                }).ToList(),
                VictoriesCount = victories.Count,
                TotalEnemies = enemies.Count,
                TotalKills = totalWins,
                HighestLevelCompleted = highestCleared,
                Victories = victories
            };
        }

        public async Task<FightSessionDTO?> CreateFightSessionAsync(int playerId, int level)
        {
            try
            {
                // Validate level
                if (level < 1 || level > 10)
                {
                    _logger.LogWarning($"Invalid level {level} requested by player {playerId}");
                    return null;
                }

                // Check for existing active session
                var existingSession = await _context.FightSessions
                    .FirstOrDefaultAsync(fs => fs.PlayerId == playerId && fs.IsActive);

                if (existingSession != null)
                {
                    _logger.LogWarning($"Player {playerId} already has an active session");
                    return null;
                }

                // Get or create arena progress
                var progress = await _context.ArenaProgressNews
                    .FirstOrDefaultAsync(ap => ap.PlayerId == playerId);

                if (progress == null)
                {
                    progress = new ArenaProgressNew { PlayerId = playerId };
                    _context.ArenaProgressNews.Add(progress);
                    await _context.SaveChangesAsync();
                }

                // Validate unlock status
                if (level > progress.HighestLevelCleared + 1)
                {
                    _logger.LogWarning($"Player {playerId} attempted to access locked level {level}");
                    return null;
                }

                // Load player and enemy
                var player = await _context.Players.FindAsync(playerId);
                var enemy = await _context.Enemies.FirstOrDefaultAsync(e => e.Level == level);

                if (player == null || enemy == null)
                {
                    _logger.LogError($"Player or enemy not found for session creation");
                    return null;
                }

                // Create session
                var session = new FightSession
                {
                    FightSessionId = Guid.NewGuid().ToString(),
                    PlayerId = playerId,
                    EnemyId = enemy.EnemyId,
                    ArenaLevel = level,
                    PlayerCurrentHP = player.HP,
                    PlayerMaxHP = player.MaxHP,
                    EnemyCurrentHP = enemy.BaseHP,
                    EnemyMaxHP = enemy.BaseHP,
                    TurnNumber = 0,
                    IsActive = true,
                    SessionStartedAt = DateTime.UtcNow
                };

                _context.FightSessions.Add(session);
                await _context.SaveChangesAsync();

                // Build DTO
                return new FightSessionDTO
                {
                    SessionId = session.FightSessionId,
                    ArenaLevel = level,
                    Player = new PlayerStatsDTO
                    {
                        PlayerId = player.PlayerId,
                        CharacterName = player.CharacterName,
                        Class = player.Class,
                        CurrentHP = player.HP,
                        MaxHP = player.MaxHP,
                        CurrentMP = player.MP,
                        MaxMP = player.MaxMP,
                        AttackPower = player.AttackPower,
                        DefenseRating = player.DefenseRating,
                        Level = player.Level,
                        CritChance = player.CriticalHitChance
                    },
                    Enemy = MapEnemyToDTO(enemy, session.EnemyCurrentHP, session.EnemyMaxHP)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating fight session");
                return null;
            }
        }

        public async Task<TurnResultDTO> ProcessPlayerTurnAsync(string sessionId, int playerId, string actionType, int? itemId = null)
        {
            try
            {
                var session = await _context.FightSessions
                    .Include(fs => fs.Player)
                    .Include(fs => fs.Enemy)
                    .FirstOrDefaultAsync(fs => fs.FightSessionId == sessionId && fs.PlayerId == playerId && fs.IsActive);

                if (session == null)
                {
                    return new TurnResultDTO { Success = false, Message = "Invalid or inactive session" };
                }

                var player = session.Player;
                var enemy = session.Enemy;

                int damage = 0;
                bool isCrit = false;
                bool isMiss = false;
                string animationType = "basicAttack";
                float knockbackForce = 1.0f;
                string particleType = "impact";

                switch (actionType.ToLower())
                {
                    case "basicattack":
                        damage = CalculateBasicAttack(player.AttackPower, enemy.BaseDEF);
                        isMiss = CheckMiss(enemy.EvasionChance);
                        if (!isMiss)
                        {
                            isCrit = CheckCritical(player.CriticalHitChance);
                            if (isCrit) damage = (int)(damage * 1.75);
                        }
                        animationType = "basicAttack";
                        knockbackForce = 1.0f;
                        particleType = GetParticleTypeForClass(player.Class);
                        break;

                    case "heavystrike":
                        damage = (int)(player.AttackPower * 1.5) - enemy.BaseDEF;
                        damage = Math.Max(1, damage);
                        // Heavy strike never misses
                        isMiss = false;
                        isCrit = CheckCritical(player.CriticalHitChance * 0.5); // Lower crit chance
                        if (isCrit) damage = (int)(damage * 1.75);
                        animationType = "heavyStrike";
                        knockbackForce = 2.0f;
                        particleType = "heavy";
                        break;

                    case "special":
                        var specialResult = ProcessClassSpecial(player, session);
                        damage = specialResult.Damage;
                        isMiss = CheckMiss((int)(enemy.EvasionChance * 0.7)); // Reduced evasion
                        if (!isMiss && CheckCritical(player.CriticalHitChance))
                        {
                            isCrit = true;
                            damage = (int)(damage * 1.75);
                        }
                        animationType = "special";
                        knockbackForce = 1.5f;
                        particleType = specialResult.ParticleType;
                        break;

                    case "useitem":
                        if (!itemId.HasValue)
                        {
                            return new TurnResultDTO { Success = false, Message = "No item specified" };
                        }
                        // Item usage logic would go here
                        damage = 0;
                        animationType = "item";
                        break;

                    default:
                        return new TurnResultDTO { Success = false, Message = "Invalid action type" };
                }

                if (!isMiss)
                {
                    session.EnemyCurrentHP -= damage;
                    session.EnemyCurrentHP = Math.Max(0, session.EnemyCurrentHP);
                }

                session.TurnNumber++;
                await _context.SaveChangesAsync();

                bool isVictory = session.EnemyCurrentHP <= 0;
                VictoryRewardDTO? reward = null;

                if (isVictory)
                {
                    reward = await ProcessVictoryAsync(sessionId);
                }

                return new TurnResultDTO
                {
                    Success = true,
                    Message = isMiss ? "Attack missed!" : $"Dealt {damage} damage!",
                    PlayerDamage = damage,
                    IsCritical = isCrit,
                    IsMiss = isMiss,
                    EnemyHPRemaining = session.EnemyCurrentHP,
                    EnemyMaxHP = session.EnemyMaxHP,
                    EnemyHPPercent = (double)session.EnemyCurrentHP / session.EnemyMaxHP,
                    IsVictory = isVictory,
                    VictoryReward = reward,
                    AnimationType = animationType,
                    KnockbackForce = knockbackForce,
                    ParticleType = particleType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing player turn");
                return new TurnResultDTO { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<EnemyTurnResultDTO> ProcessEnemyTurnAsync(string sessionId, int playerId)
        {
            try
            {
                var session = await _context.FightSessions
                    .Include(fs => fs.Player)
                    .Include(fs => fs.Enemy)
                    .FirstOrDefaultAsync(fs => fs.FightSessionId == sessionId && fs.PlayerId == playerId && fs.IsActive);

                if (session == null)
                {
                    return new EnemyTurnResultDTO { Success = false, Message = "Invalid or inactive session" };
                }

                var enemy = session.Enemy;
                double enemyHPPercent = (double)session.EnemyCurrentHP / session.EnemyMaxHP;

                // AI Phase determination
                string abilityUsed = "BasicAttack";
                float damageMultiplier = 1.0f;
                string animationType = "attack1";
                float screenShake = 0.0f;
                string particleType = enemy.ParticleTheme;

                if (enemyHPPercent > 0.66)
                {
                    // Phase 1: Basic attacks only
                    abilityUsed = "BasicAttack";
                    animationType = "attack1";
                }
                else if (enemyHPPercent > 0.33)
                {
                    // Phase 2: Alternate basic and special
                    if (session.TurnNumber % 2 == 0)
                    {
                        abilityUsed = enemy.SpecialAbilityName;
                        damageMultiplier = enemy.SpecialAbilityDamageMultiplier;
                        animationType = "special";
                        screenShake = enemy.IsBoss ? 1.0f : 0.5f;
                    }
                    else
                    {
                        abilityUsed = "BasicAttack";
                        animationType = "attack2";
                    }
                }
                else
                {
                    // Phase 3: Spam special
                    abilityUsed = enemy.SpecialAbilityName;
                    damageMultiplier = enemy.SpecialAbilityDamageMultiplier;
                    animationType = "special";
                    screenShake = enemy.IsBoss ? 1.0f : 0.5f;
                }

                // Calculate damage
                int baseDamage = enemy.BaseATK + session.EnemyATKModifier;
                int damage = (int)(baseDamage * damageMultiplier) - session.Player.DefenseRating;
                damage += _random.Next(-3, 4); // Variance
                damage = Math.Max(1, damage);

                // Apply bleed damage if active
                int bleedDamage = 0;
                if (session.BleedTurnsRemaining > 0)
                {
                    bleedDamage = session.BleedDamagePerTurn;
                    session.BleedTurnsRemaining--;
                    damage += bleedDamage;
                }

                // Reset Warrior taunt if it was active
                if (session.EnemyATKModifier != 0)
                {
                    session.EnemyATKModifier = 0; // Reset after one turn
                }

                session.PlayerCurrentHP -= damage;
                session.PlayerCurrentHP = Math.Max(0, session.PlayerCurrentHP);

                bool isDefeat = session.PlayerCurrentHP <= 0;

                if (isDefeat)
                {
                    await ProcessDefeatAsync(sessionId);
                }

                await _context.SaveChangesAsync();

                return new EnemyTurnResultDTO
                {
                    Success = true,
                    Message = $"{enemy.Name} used {abilityUsed}!",
                    EnemyDamage = damage,
                    AbilityUsed = abilityUsed,
                    AbilityName = abilityUsed,
                    PlayerHPRemaining = session.PlayerCurrentHP,
                    PlayerMaxHP = session.PlayerMaxHP,
                    PlayerHPPercent = (double)session.PlayerCurrentHP / session.PlayerMaxHP,
                    IsDefeat = isDefeat,
                    AnimationType = animationType,
                    KnockbackForce = damageMultiplier,
                    ScreenShakeIntensity = screenShake,
                    ParticleType = particleType,
                    LightFlashColor = enemy.LightingTint,
                    BleedDamage = bleedDamage
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing enemy turn");
                return new EnemyTurnResultDTO { Success = false, Message = "An error occurred" };
            }
        }

        public async Task<VictoryRewardDTO?> ProcessVictoryAsync(string sessionId)
        {
            try
            {
                var session = await _context.FightSessions
                    .Include(fs => fs.Player)
                    .Include(fs => fs.Enemy)
                    .FirstOrDefaultAsync(fs => fs.FightSessionId == sessionId);

                if (session == null) return null;

                var enemy = session.Enemy;
                var player = session.Player;

                // Calculate rewards
                int goldEarned = _random.Next(enemy.DropGoldMin, enemy.DropGoldMax + 1);
                int xpEarned = enemy.DropXP;

                player.Gold += goldEarned;
                player.XP += xpEarned;

                // Check level up
                bool leveledUp = false;
                int newLevel = player.Level;
                int xpForNextLevel = player.Level * 100;

                while (player.XP >= xpForNextLevel)
                {
                    player.Level++;
                    player.XP -= xpForNextLevel;
                    xpForNextLevel = player.Level * 100;
                    leveledUp = true;

                    // Stat increases
                    player.Strength += 2;
                    player.Agility += 2;
                    player.Intelligence += 2;
                    player.Endurance += 2;
                    player.Luck += 1;

                    // Recalculate derived stats
                    RecalculatePlayerStats(player);
                }

                newLevel = player.Level;

                // Item drop
                Item? droppedItem = null;
                int dropRoll = _random.Next(0, 100);
                if (dropRoll < enemy.DropItemChance)
                {
                    var availableItems = await _context.Items
                        .Where(i => i.Category != "Consumable")
                        .ToListAsync();

                    if (availableItems.Any())
                    {
                        droppedItem = availableItems[_random.Next(availableItems.Count)];
                        
                        // Add to inventory
                        _context.Inventories.Add(new Inventory
                        {
                            PlayerId = player.PlayerId,
                            ItemId = droppedItem.ItemId,
                            Quantity = 1,
                            IsEquipped = false,
                            AcquiredAt = DateTime.UtcNow
                        });
                    }
                }

                // Update arena progress
                var progress = await _context.ArenaProgressNews.FirstAsync(ap => ap.PlayerId == player.PlayerId);
                progress.TotalWins++;
                progress.HighestLevelCleared = Math.Max(progress.HighestLevelCleared, session.ArenaLevel);
                progress.LastFightAt = DateTime.UtcNow;

                // Log fight
                _context.FightLogNews.Add(new FightLogNew
                {
                    PlayerId = player.PlayerId,
                    EnemyId = enemy.EnemyId,
                    ArenaLevel = session.ArenaLevel,
                    Outcome = "Victory",
                    TurnsPlayed = session.TurnNumber,
                    XPEarned = xpEarned,
                    GoldEarned = goldEarned,
                    DroppedItemId = droppedItem?.ItemId,
                    FoughtAt = DateTime.UtcNow
                });

                // Deactivate session
                session.IsActive = false;

                await _context.SaveChangesAsync();

                return new VictoryRewardDTO
                {
                    GoldEarned = goldEarned,
                    XPEarned = xpEarned,
                    DroppedItemName = droppedItem?.Name,
                    DroppedItemId = droppedItem?.ItemId,
                    LeveledUp = leveledUp,
                    NewLevel = newLevel,
                    NewArenaLevel = progress.HighestLevelCleared + 1
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing victory");
                return null;
            }
        }

        public async Task ProcessDefeatAsync(string sessionId)
        {
            try
            {
                var session = await _context.FightSessions
                    .Include(fs => fs.Player)
                    .Include(fs => fs.Enemy)
                    .FirstOrDefaultAsync(fs => fs.FightSessionId == sessionId);

                if (session == null) return;

                // Update progress
                var progress = await _context.ArenaProgressNews.FirstAsync(ap => ap.PlayerId == session.PlayerId);
                progress.TotalLosses++;
                progress.LastFightAt = DateTime.UtcNow;

                // Log fight
                _context.FightLogNews.Add(new FightLogNew
                {
                    PlayerId = session.PlayerId,
                    EnemyId = session.EnemyId,
                    ArenaLevel = session.ArenaLevel,
                    Outcome = "Defeat",
                    TurnsPlayed = session.TurnNumber,
                    XPEarned = 0,
                    GoldEarned = 0,
                    FoughtAt = DateTime.UtcNow
                });

                // Deactivate session
                session.IsActive = false;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing defeat");
            }
        }

        public async Task<bool> AbandonSessionAsync(string sessionId, int playerId)
        {
            try
            {
                var session = await _context.FightSessions
                    .FirstOrDefaultAsync(fs => fs.FightSessionId == sessionId && fs.PlayerId == playerId);

                if (session == null || !session.IsActive) return false;

                await ProcessDefeatAsync(sessionId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error abandoning session");
                return false;
            }
        }

        // Helper Methods

        private int CalculateBasicAttack(int playerATK, int enemyDEF)
        {
            int damage = playerATK - enemyDEF;
            damage += _random.Next(-5, 6); // Variance
            return Math.Max(1, damage);
        }

        private bool CheckMiss(int evasionChance)
        {
            return _random.Next(0, 100) < evasionChance;
        }

        private bool CheckCritical(double critChance)
        {
            critChance = Math.Min(0.40, critChance); // Cap at 40%
            return _random.NextDouble() < critChance;
        }

        private (int Damage, string ParticleType) ProcessClassSpecial(Player player, FightSession session)
        {
            int damage = 0;
            string particleType = "special";

            switch (player.Class.ToLower())
            {
                case "warrior":
                    // Taunt: Damage + reduce enemy ATK
                    damage = (int)(player.AttackPower * 1.3);
                    session.EnemyATKModifier = (int)(session.Enemy.BaseATK * -0.15);
                    particleType = "warrior-taunt";
                    break;

                case "mage":
                    // Arcane damage ignoring half defense
                    damage = (int)((player.AttackPower - (session.Enemy.BaseDEF * 0.5)) * 1.4);
                    particleType = "arcane";
                    break;

                case "rogue":
                    // Bleed damage over time
                    damage = (int)(player.AttackPower * 1.4);
                    session.BleedTurnsRemaining = 3;
                    session.BleedDamagePerTurn = (int)(damage * 0.08);
                    particleType = "bleed";
                    break;

                case "paladin":
                    // Damage + heal
                    damage = (int)(player.AttackPower * 1.2);
                    int healAmount = (int)(player.MaxHP * 0.12);
                    session.PlayerCurrentHP = Math.Min(session.PlayerMaxHP, session.PlayerCurrentHP + healAmount);
                    particleType = "holy";
                    break;

                default:
                    damage = (int)(player.AttackPower * 1.3);
                    break;
            }

            return (Math.Max(1, damage), particleType);
        }

        private string GetParticleTypeForClass(string playerClass)
        {
            return playerClass.ToLower() switch
            {
                "warrior" => "slash",
                "mage" => "arcane",
                "rogue" => "shadow",
                "paladin" => "holy",
                _ => "impact"
            };
        }

        private void RecalculatePlayerStats(Player player)
        {
            player.MaxHP = 100 + (player.Endurance * 10) + (player.Level * 20);
            player.MaxMP = 50 + (player.Intelligence * 5) + (player.Level * 10);
            player.AttackPower = player.Strength * 2 + player.Agility;
            player.DefenseRating = player.Endurance + (player.Agility / 2);
            player.CriticalHitChance = 0.05 + (player.Luck * 0.01);

            // Heal to full on level up
            player.HP = player.MaxHP;
            player.MP = player.MaxMP;
        }

        private EnemyDataDTO MapEnemyToDTO(Enemy enemy, int currentHP, int maxHP)
        {
            return new EnemyDataDTO
            {
                EnemyId = enemy.EnemyId,
                Name = enemy.Name,
                Level = enemy.Level,
                IsBoss = enemy.IsBoss,
                CurrentHP = currentHP,
                MaxHP = maxHP,
                BaseATK = enemy.BaseATK,
                BaseDEF = enemy.BaseDEF,
                EvasionChance = enemy.EvasionChance,
                SpriteSheetPath = enemy.SpriteSheetPath,
                FrameWidth = enemy.FrameWidth,
                FrameHeight = enemy.FrameHeight,
                AnimationFPS = enemy.AnimationFPS,
                IdleFrames = enemy.IdleFrames,
                WalkFrames = enemy.WalkFrames,
                Attack1Frames = enemy.Attack1Frames,
                Attack2Frames = enemy.Attack2Frames,
                HeavyFrames = enemy.HeavyFrames,
                SpecialFrames = enemy.SpecialFrames,
                HitFrames = enemy.HitFrames,
                StaggerFrames = enemy.StaggerFrames,
                KnockbackFrames = enemy.KnockbackFrames,
                DeathFrames = enemy.DeathFrames,
                RenderScale = enemy.RenderScale,
                GroundShadowWidth = enemy.GroundShadowWidth,
                SpecialAbilityName = enemy.SpecialAbilityName,
                SpecialAbilityDamageMultiplier = enemy.SpecialAbilityDamageMultiplier,
                LightingTint = enemy.LightingTint,
                ParticleTheme = enemy.ParticleTheme
            };
        }
    }
}
