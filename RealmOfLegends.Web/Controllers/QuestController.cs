using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Data.Context;

namespace RealmOfLegends.Web.Controllers
{
    [Authorize]
    public class QuestController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GameDbContext _context;

        public QuestController(UserManager<ApplicationUser> userManager, GameDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var player = await _context.Players
                .Include(p => p.PlayerQuests)
                    .ThenInclude(pq => pq.Quest)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (player == null) return RedirectToAction("CreateCharacter", "Account");

            // Get all available quests
            var allQuests = await _context.Quests
                .OrderBy(q => q.QuestId)
                .ToListAsync();

            // Get player's quest progress
            var playerQuestIds = player.PlayerQuests.Select(pq => pq.QuestId).ToList();

            ViewBag.Player = player;
            ViewBag.AllQuests = allQuests;
            ViewBag.PlayerQuestIds = playerQuestIds;
            ViewBag.PlayerQuests = player.PlayerQuests;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartQuest(int questId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "User not found" });

            var player = await _context.Players
                .Include(p => p.PlayerQuests)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (player == null) return Json(new { success = false, message = "Player not found" });

            var quest = await _context.Quests.FindAsync(questId);
            if (quest == null) return Json(new { success = false, message = "Quest not found" });

            // Check if already started
            if (player.PlayerQuests.Any(pq => pq.QuestId == questId))
                return Json(new { success = false, message = "Quest already started" });

            // Create player quest
            var playerQuest = new PlayerQuest
            {
                PlayerId = player.PlayerId,
                QuestId = questId,
                Status = "InProgress",
                AssignedAt = DateTime.UtcNow
            };

            _context.PlayerQuests.Add(playerQuest);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Quest started!" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteQuest(int questId, int score = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "User not found" });

            var player = await _context.Players
                .Include(p => p.PlayerQuests)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (player == null) return Json(new { success = false, message = "Player not found" });

            var playerQuest = player.PlayerQuests.FirstOrDefault(pq => pq.QuestId == questId && pq.Status == "InProgress");
            if (playerQuest == null) return Json(new { success = false, message = "Quest not in progress" });

            var quest = await _context.Quests.FindAsync(questId);
            if (quest == null) return Json(new { success = false, message = "Quest not found" });

            // Mark quest as completed
            playerQuest.Status = "Completed";
            playerQuest.CompletedAt = DateTime.UtcNow;

            // Award rewards
            player.Gold += quest.RewardGold;
            player.XP += quest.RewardXP;

            // Check for level up
            var xpForNextLevel = player.Level * 1000;
            if (player.XP >= xpForNextLevel)
            {
                player.Level++;
                player.XP -= xpForNextLevel;
                
                // Increase stats on level up
                player.Strength += 2;
                player.Agility += 2;
                player.Intelligence += 2;
                player.Endurance += 2;
                player.Luck += 1;
                
                player.MaxHP += 10;
                player.MaxMP += 5;
                player.HP = player.MaxHP;
                player.MP = player.MaxMP;
                player.AttackPower += 2;
                player.DefenseRating += 1;
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Quest completed!",
                rewards = new
                {
                    gold = quest.RewardGold,
                    xp = quest.RewardXP
                },
                player = new
                {
                    gold = player.Gold,
                    xp = player.XP,
                    level = player.Level,
                    hp = player.HP,
                    maxHp = player.MaxHP
                }
            });
        }

        public IActionResult ClickChallenge()
        {
            return View();
        }

        public IActionResult MemoryMatch()
        {
            return View();
        }

        public IActionResult Riddle()
        {
            return View();
        }

        public IActionResult SliderPuzzle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitClickChallenge(int clicks, double duration)
        {
            // Validate click speed (prevent cheating)
            var clicksPerSecond = clicks / duration;
            if (clicksPerSecond > 20) // Max 20 clicks per second
            {
                return Json(new { success = false, message = "Suspicious click speed detected" });
            }

            // Find Click Challenge quest
            var quest = await _context.Quests.FirstOrDefaultAsync(q => q.Title == "Click Challenge");
            if (quest != null)
            {
                return await CompleteQuest(quest.QuestId, (int)clicks);
            }

            return Json(new { success = false, message = "Quest not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitMemoryMatch(int moves, double time)
        {
            // Validate moves
            if (moves < 8) // Minimum 8 moves to complete 4x4 grid
            {
                return Json(new { success = false, message = "Invalid moves count" });
            }

            var quest = await _context.Quests.FirstOrDefaultAsync(q => q.Title == "Memory Match");
            if (quest != null)
            {
                return await CompleteQuest(quest.QuestId);
            }

            return Json(new { success = false, message = "Quest not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRiddle(int riddleId, string answer)
        {
            // Simple riddle validation (can expand with more riddles)
            var riddles = new Dictionary<int, string>
            {
                { 1, "echo" },
                { 2, "map" },
                { 3, "fire" }
            };

            if (!riddles.ContainsKey(riddleId))
            {
                return Json(new { success = false, message = "Invalid riddle" });
            }

            if (!string.Equals(answer?.Trim(), riddles[riddleId], StringComparison.OrdinalIgnoreCase))
            {
                return Json(new { success = false, message = "Wrong answer! Try again." });
            }

            var quest = await _context.Quests.FirstOrDefaultAsync(q => q.Title == "Riddle Master");
            if (quest != null)
            {
                return await CompleteQuest(quest.QuestId);
            }

            return Json(new { success = false, message = "Quest not found" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitSliderPuzzle(int moves, double time)
        {
            // Validate completion
            if (moves < 10) // Minimum moves for 3x3 puzzle
            {
                return Json(new { success = false, message = "Invalid puzzle solution" });
            }

            var quest = await _context.Quests.FirstOrDefaultAsync(q => q.Title == "Slider Puzzle");
            if (quest != null)
            {
                return await CompleteQuest(quest.QuestId);
            }

            return Json(new { success = false, message = "Quest not found" });
        }
    }
}
