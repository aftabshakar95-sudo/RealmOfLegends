using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Core.Entities.Arena;
using RealmOfLegends.Core.Services.Arena;
using RealmOfLegends.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace RealmOfLegends.Web.Controllers
{
    [Authorize]
    [Route("ArenaCanvas")]
    public class ArenaControllerNew : Controller
    {
        private readonly IArenaService _arenaService;
        private readonly GameDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ArenaControllerNew> _logger;

        public ArenaControllerNew(
            IArenaService arenaService,
            GameDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<ArenaControllerNew> logger)
        {
            _arenaService = arenaService;
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return RedirectToAction("Create", "Account");
                }

                // Get hub data using service
                var hubData = await _arenaService.GetArenaHubDataAsync(player.PlayerId);

                return View("~/Views/ArenaCanvas/Index.cshtml", hubData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading arena index");
                return RedirectToAction("Index", "Dashboard");
            }
        }

        [HttpGet("Fight")]
        public async Task<IActionResult> Fight(int enemyId, int levelId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return RedirectToAction("Create", "Account");
                }

                ViewData["EnemyId"] = enemyId;
                ViewData["LevelId"] = levelId;

                return View("~/Views/ArenaCanvas/FightSimple.cshtml");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading fight view");
                return RedirectToAction("Index");
            }
        }

        // API Endpoints for Canvas Combat
        [HttpGet("/api/arena/player-stats")]
        public async Task<IActionResult> GetPlayerStats()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return NotFound(new { error = "Player not found" });
                }

                return Json(new
                {
                    name = player.CharacterName,
                    className = player.Class,
                    level = player.Level,
                    health = player.MaxHP,
                    attack = player.AttackPower,
                    defense = player.DefenseRating
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting player stats");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("/api/arena/enemy/{id}")]
        public async Task<IActionResult> GetEnemyStats(int id)
        {
            try
            {
                var enemy = await _context.Enemies.FindAsync(id);

                if (enemy == null)
                {
                    return NotFound(new { error = "Enemy not found" });
                }

                return Json(new
                {
                    id = enemy.EnemyId,
                    name = enemy.Name,
                    level = enemy.Level,
                    health = enemy.BaseHP,
                    attack = enemy.BaseATK,
                    defense = enemy.BaseDEF,
                    isBoss = enemy.IsBoss
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting enemy stats");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("/api/arena/battle-result")]
        public async Task<IActionResult> BattleResult([FromBody] BattleResultModel model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return NotFound(new { error = "Player not found" });
                }

                var enemy = await _context.Enemies.FindAsync(model.EnemyId);
                if (enemy == null)
                {
                    return NotFound(new { error = "Enemy not found" });
                }

                // Update progress
                var progress = await _context.ArenaProgressNews
                    .FirstOrDefaultAsync(ap => ap.PlayerId == player.PlayerId);

                if (progress == null)
                {
                    progress = new ArenaProgressNew
                    {
                        PlayerId = player.PlayerId,
                        HighestLevelCleared = 0,
                        TotalWins = 0,
                        TotalLosses = 0
                    };
                    _context.ArenaProgressNews.Add(progress);
                }

                if (model.Victory)
                {
                    progress.TotalWins++;
                    if (enemy.Level > progress.HighestLevelCleared)
                    {
                        progress.HighestLevelCleared = enemy.Level;
                    }

                    // Grant rewards
                    player.XP += enemy.DropXP;
                    player.Gold += new Random().Next(enemy.DropGoldMin, enemy.DropGoldMax + 1);
                }
                else
                {
                    progress.TotalLosses++;
                }

                // Log the fight
                var fightLog = new FightLogNew
                {
                    PlayerId = player.PlayerId,
                    EnemyId = enemy.EnemyId,
                    ArenaLevel = enemy.Level,
                    Outcome = model.Victory ? "Victory" : "Defeat",
                    TurnsPlayed = model.TurnCount,
                    XPEarned = model.Victory ? enemy.DropXP : 0,
                    GoldEarned = model.Victory ? enemy.DropGoldMin : 0,
                    FoughtAt = DateTime.UtcNow
                };
                _context.FightLogNews.Add(fightLog);

                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving battle result");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPost("SubmitTurn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTurn([FromBody] TurnRequestModel model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return Json(new { success = false, message = "Player not found" });
                }

                var result = await _arenaService.ProcessPlayerTurnAsync(
                    model.SessionId,
                    player.PlayerId,
                    model.ActionType,
                    model.ItemId
                );

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing player turn");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpGet("EnemyTurn/{sessionId}")]
        public async Task<IActionResult> EnemyTurn(string sessionId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return Json(new { success = false, message = "Player not found" });
                }

                var result = await _arenaService.ProcessEnemyTurnAsync(sessionId, player.PlayerId);

                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing enemy turn");
                return Json(new { success = false, message = "An error occurred" });
            }
        }

        [HttpPost("Abandon")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Abandon([FromBody] AbandonRequestModel model)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return Json(new { success = false });
                }

                var success = await _arenaService.AbandonSessionAsync(model.SessionId, player.PlayerId);

                return Json(new { success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error abandoning session");
                return Json(new { success = false });
            }
        }

        [HttpGet("History")]
        public async Task<IActionResult> History()
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == userId);

                if (player == null)
                {
                    return RedirectToAction("Create", "Account");
                }

                var history = await _context.FightLogNews
                    .Where(fl => fl.PlayerId == player.PlayerId)
                    .Include(fl => fl.Enemy)
                    .Include(fl => fl.DroppedItem)
                    .OrderByDescending(fl => fl.FoughtAt)
                    .Take(20)
                    .ToListAsync();

                ViewBag.History = history;
                ViewBag.Player = player;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading fight history");
                return RedirectToAction("Index");
            }
        }
    }

    // Request Models
    public class TurnRequestModel
    {
        public string SessionId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public int? ItemId { get; set; }
    }

    public class AbandonRequestModel
    {
        public string SessionId { get; set; } = string.Empty;
    }

    public class BattleResultModel
    {
        public int EnemyId { get; set; }
        public bool Victory { get; set; }
        public int TurnCount { get; set; }
        public int DamageDealt { get; set; }
        public int DamageTaken { get; set; }
    }
}
