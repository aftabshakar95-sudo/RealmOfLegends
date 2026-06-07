using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Data.Context;
using RealmOfLegends.Web.Models.ViewModels;

namespace RealmOfLegends.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly GameDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            GameDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Generate email confirmation token
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                
                // TODO: In production, send confirmation email
                // For development, auto-confirm the email
                if (user.Id != null)
                {
                    await _userManager.ConfirmEmailAsync(user, token);
                    user.EmailConfirmedAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }

                // Redirect to character creation
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(CreateCharacter));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Check if user has a character
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var player = await _context.Players.FirstOrDefaultAsync(p => p.UserId == user.Id);
                    
                    if (player == null)
                    {
                        // No character created yet
                        return RedirectToAction(nameof(CreateCharacter));
                    }

                    // Update last login
                    player.LastLoginAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account is locked due to multiple failed login attempts. Please try again in 15 minutes.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        // GET: /Account/CreateCharacter
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreateCharacter()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Check if character already exists
            var existingPlayer = await _context.Players.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (existingPlayer != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        // POST: /Account/CreateCharacter
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCharacter(CreateCharacterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction(nameof(Login));
                }

                // Check if character already exists
                var existingPlayer = await _context.Players.FirstOrDefaultAsync(p => p.UserId == user.Id);
                if (existingPlayer != null)
                {
                    return RedirectToAction("Index", "Dashboard");
                }

                // Validate class
                var validClasses = new[] { "Warrior", "Mage", "Rogue", "Paladin" };
                if (string.IsNullOrEmpty(model.Class) || !validClasses.Contains(model.Class))
                {
                    ModelState.AddModelError(nameof(model.Class), "Invalid class selection.");
                    return View(model);
                }

                // Generate default character name if not provided
                var characterName = string.IsNullOrWhiteSpace(model.CharacterName)
                    ? $"{model.Class}_{user.UserName?.Substring(0, Math.Min(user.UserName?.Length ?? 0, 10))}"
                    : model.CharacterName.Trim();

                // Create player with class-specific stats
                var player = new Player
                {
                    UserId = user.Id,
                    CharacterName = characterName,
                    Class = model.Class,
                    Level = 1,
                    XP = 0,
                    Gold = 100, // Starting gold
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow
                };

                // Set class-specific base stats
                switch (model.Class)
                {
                    case "Warrior":
                        player.Strength = 15;
                        player.Agility = 8;
                        player.Intelligence = 5;
                        player.Endurance = 14;
                        player.Luck = 8;
                        player.HP = 150;
                        player.MaxHP = 150;
                        player.MP = 30;
                        player.MaxMP = 30;
                        player.AttackPower = 18;
                        player.DefenseRating = 12;
                        player.CriticalHitChance = 0.08;
                        break;
                    case "Mage":
                        player.Strength = 5;
                        player.Agility = 8;
                        player.Intelligence = 16;
                        player.Endurance = 7;
                        player.Luck = 9;
                        player.HP = 80;
                        player.MaxHP = 80;
                        player.MP = 150;
                        player.MaxMP = 150;
                        player.AttackPower = 22;
                        player.DefenseRating = 5;
                        player.CriticalHitChance = 0.12;
                        break;
                    case "Rogue":
                        player.Strength = 10;
                        player.Agility = 16;
                        player.Intelligence = 8;
                        player.Endurance = 8;
                        player.Luck = 13;
                        player.HP = 100;
                        player.MaxHP = 100;
                        player.MP = 50;
                        player.MaxMP = 50;
                        player.AttackPower = 16;
                        player.DefenseRating = 7;
                        player.CriticalHitChance = 0.25;
                        break;
                    case "Paladin":
                        player.Strength = 12;
                        player.Agility = 9;
                        player.Intelligence = 10;
                        player.Endurance = 12;
                        player.Luck = 10;
                        player.HP = 130;
                        player.MaxHP = 130;
                        player.MP = 80;
                        player.MaxMP = 80;
                        player.AttackPower = 15;
                        player.DefenseRating = 10;
                        player.CriticalHitChance = 0.10;
                        break;
                }

                _context.Players.Add(player);

                // Create initial ArenaProgress
                var arenaProgress = new ArenaProgress
                {
                    Player = player,
                    CurrentLevel = 1,
                    HighestLevel = 1,
                    TotalWins = 0,
                    TotalLosses = 0
                };
                _context.ArenaProgresses.Add(arenaProgress);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Dashboard");
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty, "Database error occurred while creating your character. Please try again.");
                // Log the exception (you can add logging here)
                return View(model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                // Log the exception (you can add logging here)
                return View(model);
            }
        }

        // POST: /Account/Logout
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
