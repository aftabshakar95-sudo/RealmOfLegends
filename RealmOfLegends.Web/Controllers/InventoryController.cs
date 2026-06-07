using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Data.Context;

namespace RealmOfLegends.Web.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly GameDbContext _context;

        public InventoryController(UserManager<ApplicationUser> userManager, GameDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(string category = "All")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var player = await _context.Players
                .Include(p => p.Inventories)
                    .ThenInclude(i => i.Item)
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (player == null) return RedirectToAction("CreateCharacter", "Account");

            var itemsQuery = player.Inventories.AsQueryable();

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                itemsQuery = itemsQuery.Where(i => i.Item.Category == category);
            }

            var items = itemsQuery.OrderByDescending(i => i.Item.Rarity).ThenBy(i => i.Item.Name).ToList();

            // Calculate total stats from equipped items
            var equippedItems = player.Inventories.Where(i => i.IsEquipped).ToList();
            int bonusAtk = 0, bonusDef = 0;
            
            foreach(var inv in equippedItems)
            {
                if(!string.IsNullOrEmpty(inv.Item.StatJson) && inv.Item.StatJson != "{}")
                {
                    try
                    {
                        var stats = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(inv.Item.StatJson);
                        if (stats != null)
                        {
                            if (stats.ContainsKey("ATK")) bonusAtk += stats["ATK"];
                            if (stats.ContainsKey("DEF")) bonusDef += stats["DEF"];
                        }
                    }
                    catch { /* ignore invalid json */ }
                }
            }

            ViewBag.Player = player;
            ViewBag.Category = category;
            ViewBag.BonusAtk = bonusAtk;
            ViewBag.BonusDef = bonusDef;

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Equip(int inventoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (player == null) return NotFound();

            var itemToEquip = await _context.Inventories
                .Include(i => i.Item)
                .FirstOrDefaultAsync(i => i.InventoryId == inventoryId && i.PlayerId == player.PlayerId);

            if (itemToEquip == null) return NotFound("Item not found in inventory.");

            // Find if there is already an item equipped in this slot
            var currentlyEquipped = await _context.Inventories
                .FirstOrDefaultAsync(i => i.PlayerId == player.PlayerId 
                                       && i.IsEquipped 
                                       && i.SlotType == itemToEquip.Item.SlotType);

            if (currentlyEquipped != null)
            {
                currentlyEquipped.IsEquipped = false;
            }

            itemToEquip.IsEquipped = true;
            itemToEquip.SlotType = itemToEquip.Item.SlotType; // Ensure slot matches the item's defined slot
            
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Equipped {itemToEquip.Item.Name}!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unequip(int inventoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (player == null) return NotFound();

            var itemToUnequip = await _context.Inventories
                .Include(i => i.Item)
                .FirstOrDefaultAsync(i => i.InventoryId == inventoryId && i.PlayerId == player.PlayerId);

            if (itemToUnequip == null || !itemToUnequip.IsEquipped) return BadRequest("Item not found or not equipped.");

            itemToUnequip.IsEquipped = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Unequipped {itemToUnequip.Item.Name}!";
            return RedirectToAction(nameof(Index));
        }
    }
}
