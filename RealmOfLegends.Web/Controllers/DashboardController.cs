using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Data.Context;

namespace RealmOfLegends.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GameDbContext _context;

        public DashboardController(UserManager<ApplicationUser> userManager, GameDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var player = await _context.Players
                .Include(p => p.ArenaProgress)
                .Include(p => p.PlayerAchievements)
                    .ThenInclude(pa => pa.Achievement)
                .Include(p => p.Inventories.Where(i => i.IsEquipped))
                    .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (player == null)
            {
                return RedirectToAction("CreateCharacter", "Account");
            }

            // Get leaderboard data
            var leaderboard = await _context.Players
                .Include(p => p.ArenaProgress)
                .OrderByDescending(p => p.ArenaProgress != null ? p.ArenaProgress.HighestLevel : 0)
                .ThenByDescending(p => p.Gold)
                .Take(10)
                .Select(p => new
                {
                    p.CharacterName,
                    p.Class,
                    p.Level,
                    p.Gold,
                    ArenaLevel = p.ArenaProgress != null ? p.ArenaProgress.HighestLevel : 0,
                    IsCurrentPlayer = p.PlayerId == player.PlayerId
                })
                .ToListAsync();

            // Get today's stats
            var today = DateTime.UtcNow.Date;
            var questsToday = await _context.PlayerQuests
                .Where(pq => pq.PlayerId == player.PlayerId && 
                            pq.CompletedAt.HasValue && 
                            pq.CompletedAt.Value.Date == today)
                .CountAsync();

            var goldToday = await _context.PurchaseHistories
                .Where(ph => ph.PlayerId == player.PlayerId && ph.PurchasedAt.Date == today)
                .SumAsync(ph => (int?)ph.GoldSpent) ?? 0;

            var arenaWinsToday = await _context.FightLogs
                .Where(fl => fl.PlayerId == player.PlayerId && 
                            fl.FoughtAt.Date == today && 
                            fl.Outcome == "Victory")
                .CountAsync();

            // Get latest news
            var news = await _context.News
                .Where(n => n.IsActive)
                .OrderByDescending(n => n.PostedAt)
                .Take(3)
                .ToListAsync();

            // Pass data to view
            ViewBag.Player = player;
            ViewBag.Leaderboard = leaderboard;
            ViewBag.QuestsToday = questsToday;
            ViewBag.GoldToday = goldToday;
            ViewBag.ArenaWinsToday = arenaWinsToday;
            ViewBag.News = news;
            ViewBag.PlayerGold = player.Gold;
            ViewBag.PlayerLevel = player.Level;
            ViewBag.CharacterName = player.CharacterName;
            ViewBag.PlayerClass = player.Class;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetLeaderboard()
        {
            var leaderboard = await _context.Players
                .Include(p => p.ArenaProgress)
                .OrderByDescending(p => p.ArenaProgress != null ? p.ArenaProgress.HighestLevel : 0)
                .ThenByDescending(p => p.Gold)
                .Take(10)
                .Select(p => new
                {
                    rank = 0,
                    name = p.CharacterName,
                    @class = p.Class,
                    level = p.Level,
                    gold = p.Gold,
                    arenaLevel = p.ArenaProgress != null ? p.ArenaProgress.HighestLevel : 0
                })
                .ToListAsync();

            // Add ranks
            var rankedLeaderboard = leaderboard.Select((p, index) => new
            {
                rank = index + 1,
                p.name,
                p.@class,
                p.level,
                p.gold,
                p.arenaLevel
            });

            return Json(rankedLeaderboard);
        }
    }
}
