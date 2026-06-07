using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Data.Context;
using System.Text.Json;

namespace RealmOfLegends.Web.Controllers
{
    [Authorize]
    public class ArenaController : Controller
    {
        private readonly GameDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ArenaController> _logger;

        public ArenaController(
            GameDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ArenaController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players
                    .Include(p => p.ArenaProgress)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return RedirectToAction("Create", "Account");
                }

                // Ensure arena progress exists
                if (player.ArenaProgress == null)
                {
                    player.ArenaProgress = new ArenaProgress
                    {
                        PlayerId = player.PlayerId,
                        CurrentLevel = 1,
                        HighestLevel = 1,
                        TotalWins = 0,
                        TotalLosses = 0
                    };
                    _context.ArenaProgresses.Add(player.ArenaProgress);
                    await _context.SaveChangesAsync();
                }

                // Get recent fight logs
                var recentFights = await _context.FightLogs
                    .Where(f => f.PlayerId == player.PlayerId)
                    .OrderByDescending(f => f.FoughtAt)
                    .Take(10)
                    .Include(f => f.DroppedItem)
                    .ToListAsync();

                ViewBag.Player = player;
                ViewBag.RecentFights = recentFights;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading arena");
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Fight()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players
                    .Include(p => p.ArenaProgress)
                    .Include(p => p.Inventories)
                        .ThenInclude(i => i.Item)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null || player.ArenaProgress == null)
                {
                    return Json(new { success = false, message = "Player not found" });
                }

                // Calculate player stats including equipped items
                var equippedItems = player.Inventories.Where(i => i.IsEquipped).Select(i => i.Item).ToList();
                int playerPower = CalculatePlayerPower(player, equippedItems);
                
                int currentLevel = player.ArenaProgress.CurrentLevel;
                int enemyPower = CalculateEnemyPower(currentLevel);
                
                // Determine outcome with some randomness
                Random rng = new Random();
                double playerChance = (double)playerPower / (playerPower + enemyPower);
                playerChance = Math.Max(0.1, Math.Min(0.9, playerChance)); // Clamp between 10% and 90%
                
                bool victory = rng.NextDouble() < playerChance;
                
                // Calculate rewards
                int xpEarned = 0;
                int goldEarned = 0;
                Item? droppedItem = null;
                string enemyName = GetEnemyName(currentLevel);
                
                if (victory)
                {
                    xpEarned = currentLevel * 50 + rng.Next(10, 30);
                    goldEarned = currentLevel * 20 + rng.Next(5, 15);
                    
                    // Chance to drop item
                    if (rng.NextDouble() < 0.3) // 30% drop chance
                    {
                        var availableItems = await _context.Items
                            .Where(i => i.Category != "Consumable")
                            .ToListAsync();
                        
                        if (availableItems.Any())
                        {
                            droppedItem = availableItems[rng.Next(availableItems.Count)];
                        }
                    }
                    
                    // Update player stats
                    player.XP += xpEarned;
                    player.Gold += goldEarned;
                    
                    // Check for level up
                    int xpForNextLevel = player.Level * 100;
                    while (player.XP >= xpForNextLevel)
                    {
                        player.Level++;
                        player.XP -= xpForNextLevel;
                        xpForNextLevel = player.Level * 100;
                        
                        // Increase base stats on level up
                        player.Strength += 2;
                        player.Agility += 2;
                        player.Intelligence += 2;
                        player.Endurance += 2;
                        player.Luck += 1;
                        
                        RecalculatePlayerStats(player, equippedItems);
                    }
                    
                    // Add dropped item to inventory
                    if (droppedItem != null)
                    {
                        player.Inventories.Add(new Inventory
                        {
                            PlayerId = player.PlayerId,
                            ItemId = droppedItem.ItemId,
                            Quantity = 1,
                            IsEquipped = false,
                            AcquiredAt = DateTime.UtcNow
                        });
                    }
                    
                    // Update arena progress
                    player.ArenaProgress.TotalWins++;
                    player.ArenaProgress.CurrentLevel = Math.Min(10, currentLevel + 1);
                    player.ArenaProgress.HighestLevel = Math.Max(player.ArenaProgress.HighestLevel, player.ArenaProgress.CurrentLevel);
                    player.ArenaProgress.LastFightAt = DateTime.UtcNow;
                }
                else
                {
                    // Defeat - small consolation reward
                    xpEarned = currentLevel * 10;
                    goldEarned = currentLevel * 5;
                    
                    player.XP += xpEarned;
                    player.Gold += goldEarned;
                    
                    // Update arena progress
                    player.ArenaProgress.TotalLosses++;
                    player.ArenaProgress.CurrentLevel = Math.Max(1, currentLevel - 1);
                    player.ArenaProgress.LastFightAt = DateTime.UtcNow;
                }
                
                // Log the fight
                var fightLog = new FightLog
                {
                    PlayerId = player.PlayerId,
                    Level = currentLevel,
                    Outcome = victory ? "Victory" : "Defeat",
                    XPEarned = xpEarned,
                    GoldEarned = goldEarned,
                    DroppedItemId = droppedItem?.ItemId,
                    FoughtAt = DateTime.UtcNow
                };
                _context.FightLogs.Add(fightLog);
                
                await _context.SaveChangesAsync();
                
                return Json(new
                {
                    success = true,
                    victory = victory,
                    enemyName = enemyName,
                    enemyLevel = currentLevel,
                    xpEarned = xpEarned,
                    goldEarned = goldEarned,
                    droppedItem = droppedItem?.Name,
                    newLevel = player.ArenaProgress.CurrentLevel,
                    playerLevel = player.Level,
                    playerXP = player.XP,
                    playerGold = player.Gold,
                    totalWins = player.ArenaProgress.TotalWins,
                    totalLosses = player.ArenaProgress.TotalLosses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during arena fight");
                return Json(new { success = false, message = "An error occurred during the fight" });
            }
        }

        private int CalculatePlayerPower(Player player, List<Item> equippedItems)
        {
            int power = player.AttackPower + player.DefenseRating;
            power += player.Strength * 2;
            power += player.Agility;
            power += player.Intelligence;
            power += player.Endurance;
            
            foreach (var item in equippedItems)
            {
                var stats = ParseItemStats(item.StatJson);
                power += stats.GetValueOrDefault("ATK", 0);
                power += stats.GetValueOrDefault("DEF", 0);
                power += stats.GetValueOrDefault("STR", 0);
                power += stats.GetValueOrDefault("AGI", 0);
            }
            
            return power;
        }
        
        private Dictionary<string, int> ParseItemStats(string statJson)
        {
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, int>>(statJson) ?? new Dictionary<string, int>();
            }
            catch
            {
                return new Dictionary<string, int>();
            }
        }

        private int CalculateEnemyPower(int level)
        {
            // Enemy power scales with level
            return 50 + (level * 30);
        }

        private string GetEnemyName(int level)
        {
            var enemies = new Dictionary<int, string>
            {
                { 1, "Goblin Scout" },
                { 2, "Wild Wolf" },
                { 3, "Orc Warrior" },
                { 4, "Dark Mage" },
                { 5, "Stone Golem" },
                { 6, "Vampire Lord" },
                { 7, "Fire Elemental" },
                { 8, "Shadow Assassin" },
                { 9, "Frost Giant" },
                { 10, "Ancient Dragon" }
            };
            
            return enemies.ContainsKey(level) ? enemies[level] : "Unknown Enemy";
        }

        private void RecalculatePlayerStats(Player player, List<Item> equippedItems)
        {
            // Base stats
            player.MaxHP = 100 + (player.Endurance * 10) + (player.Level * 20);
            player.MaxMP = 50 + (player.Intelligence * 5) + (player.Level * 10);
            player.AttackPower = player.Strength * 2 + player.Agility;
            player.DefenseRating = player.Endurance + (player.Agility / 2);
            player.CriticalHitChance = 0.05 + (player.Luck * 0.01);
            
            // Add equipment bonuses
            foreach (var item in equippedItems)
            {
                var stats = ParseItemStats(item.StatJson);
                player.MaxHP += stats.GetValueOrDefault("HP", 0);
                player.MaxMP += stats.GetValueOrDefault("MP", 0);
                player.AttackPower += stats.GetValueOrDefault("ATK", 0) + stats.GetValueOrDefault("STR", 0);
                player.DefenseRating += stats.GetValueOrDefault("DEF", 0);
            }
            
            // Heal to full
            player.HP = player.MaxHP;
            player.MP = player.MaxMP;
        }
    }
}
