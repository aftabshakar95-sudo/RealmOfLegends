using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Data.Context;

namespace RealmOfLegends.Web.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GameDbContext _context;

        public ShopController(UserManager<ApplicationUser> userManager, GameDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string? category = null, string? rarity = null, string? sort = null)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (player == null) return RedirectToAction("CreateCharacter", "Account");

            // Get all items
            var itemsQuery = _context.Items.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                itemsQuery = itemsQuery.Where(i => i.Category == category);
            }

            if (!string.IsNullOrEmpty(rarity) && rarity != "all")
            {
                itemsQuery = itemsQuery.Where(i => i.Rarity == rarity);
            }

            // Apply sorting
            itemsQuery = sort switch
            {
                "price-asc" => itemsQuery.OrderBy(i => i.Price),
                "price-desc" => itemsQuery.OrderByDescending(i => i.Price),
                "name" => itemsQuery.OrderBy(i => i.Name),
                "rarity" => itemsQuery.OrderBy(i => i.Rarity),
                _ => itemsQuery.OrderBy(i => i.ItemId)
            };

            var items = await itemsQuery.ToListAsync();

            ViewBag.Player = player;
            ViewBag.Items = items;
            ViewBag.CurrentCategory = category ?? "all";
            ViewBag.CurrentRarity = rarity ?? "all";
            ViewBag.CurrentSort = sort ?? "default";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Purchase(int itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "User not found" });

            var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (player == null) return Json(new { success = false, message = "Player not found" });

            var item = await _context.Items.FindAsync(itemId);
            if (item == null) return Json(new { success = false, message = "Item not found" });

            // Check if player has enough gold
            if (player.Gold < item.Price)
            {
                return Json(new
                {
                    success = false,
                    message = "Insufficient gold",
                    deficit = item.Price - player.Gold
                });
            }

            // Use transaction for atomic operation
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Deduct gold
                player.Gold -= item.Price;

                // Add item to inventory
                var inventory = new Inventory
                {
                    PlayerId = player.PlayerId,
                    ItemId = itemId,
                    Quantity = 1,
                    IsEquipped = false,
                    SlotType = item.SlotType,
                    AcquiredAt = DateTime.UtcNow
                };

                _context.Inventories.Add(inventory);

                // Record purchase history
                var purchase = new PurchaseHistory
                {
                    PlayerId = player.PlayerId,
                    ItemId = itemId,
                    GoldSpent = item.Price,
                    PurchasedAt = DateTime.UtcNow
                };

                _context.PurchaseHistories.Add(purchase);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "Purchase successful!",
                    item = new
                    {
                        name = item.Name,
                        rarity = item.Rarity
                    },
                    player = new
                    {
                        gold = player.Gold
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Purchase failed: " + ex.Message });
            }
        }
    }
}
