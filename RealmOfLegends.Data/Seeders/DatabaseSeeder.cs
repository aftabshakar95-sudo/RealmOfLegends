using Microsoft.EntityFrameworkCore;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Data.Context;

namespace RealmOfLegends.Data.Seeders
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(GameDbContext context)
        {
            // Always try to seed enemies (it has its own check)
            await ArenaSeeder.SeedEnemiesAsync(context);
            
            // Check if other data already seeded
            if (await context.Items.AnyAsync())
            {
                return;
            }

            await SeedItemsAsync(context);
            await SeedQuestsAsync(context);
            await SeedAchievementsAsync(context);
            await SeedNewsAsync(context);
            
            await context.SaveChangesAsync();
        }

        private static async Task SeedItemsAsync(GameDbContext context)
        {
            var items = new List<Item>
            {
                // WEAPONS - Common
                new Item { Name = "Rusty Sword", Category = "Weapon", Rarity = "Common", Price = 50, StatJson = "{\"ATK\": 5}", Description = "A worn blade that has seen better days.", ImagePath = "/images/items/rusty-sword.png", SlotType = "Weapon" },
                new Item { Name = "Wooden Staff", Category = "Weapon", Rarity = "Common", Price = 45, StatJson = "{\"ATK\": 3, \"INT\": 2}", Description = "A simple wooden staff for novice mages.", ImagePath = "/images/items/wooden-staff.png", SlotType = "Weapon" },
                new Item { Name = "Iron Dagger", Category = "Weapon", Rarity = "Common", Price = 55, StatJson = "{\"ATK\": 4, \"CritChance\": 5}", Description = "A basic dagger for quick strikes.", ImagePath = "/images/items/iron-dagger.png", SlotType = "Weapon" },
                
                // WEAPONS - Uncommon
                new Item { Name = "Steel Sword", Category = "Weapon", Rarity = "Uncommon", Price = 200, StatJson = "{\"ATK\": 15}", Description = "A well-crafted steel blade.", ImagePath = "/images/items/steel-sword.png", SlotType = "Weapon" },
                new Item { Name = "Enchanted Staff", Category = "Weapon", Rarity = "Uncommon", Price = 220, StatJson = "{\"ATK\": 10, \"INT\": 8}", Description = "A staff humming with magical energy.", ImagePath = "/images/items/enchanted-staff.png", SlotType = "Weapon" },
                new Item { Name = "Elven Bow", Category = "Weapon", Rarity = "Uncommon", Price = 210, StatJson = "{\"ATK\": 12, \"AGI\": 5}", Description = "A graceful bow from the elven forests.", ImagePath = "/images/items/elven-bow.png", SlotType = "Weapon" },
                
                // WEAPONS - Rare
                new Item { Name = "Flamebrand", Category = "Weapon", Rarity = "Rare", Price = 500, StatJson = "{\"ATK\": 30, \"CritChance\": 10}", Description = "A sword wreathed in eternal flames.", ImagePath = "/images/items/flamebrand.png", SlotType = "Weapon" },
                new Item { Name = "Stormcaller", Category = "Weapon", Rarity = "Rare", Price = 520, StatJson = "{\"ATK\": 25, \"INT\": 15}", Description = "This staff calls forth the fury of storms.", ImagePath = "/images/items/stormcaller.png", SlotType = "Weapon" },
                
                // WEAPONS - Epic
                new Item { Name = "Dragonslayer Greatsword", Category = "Weapon", Rarity = "Epic", Price = 1200, StatJson = "{\"ATK\": 55, \"STR\": 10}", Description = "Forged to slay dragons, this massive blade radiates power.", ImagePath = "/images/items/dragonslayer.png", SlotType = "Weapon" },
                new Item { Name = "Archmage's Scepter", Category = "Weapon", Rarity = "Epic", Price = 1300, StatJson = "{\"ATK\": 40, \"INT\": 25}", Description = "The legendary weapon of the Archmage order.", ImagePath = "/images/items/archmage-scepter.png", SlotType = "Weapon" },
                
                // WEAPONS - Legendary
                new Item { Name = "Sword of the Fallen King", Category = "Weapon", Rarity = "Legendary", Price = 3000, StatJson = "{\"ATK\": 100, \"STR\": 20, \"CritChance\": 25}", Description = "The cursed blade of an ancient tyrant king.", ImagePath = "/images/items/fallen-king-sword.png", SlotType = "Weapon" },
                
                // ARMOR - Head (Common to Epic)
                new Item { Name = "Leather Cap", Category = "Armour", Rarity = "Common", Price = 40, StatJson = "{\"DEF\": 3}", Description = "Basic leather headgear.", ImagePath = "/images/items/leather-cap.png", SlotType = "Head" },
                new Item { Name = "Iron Helmet", Category = "Armour", Rarity = "Uncommon", Price = 150, StatJson = "{\"DEF\": 10, \"END\": 3}", Description = "Solid iron protection for the head.", ImagePath = "/images/items/iron-helmet.png", SlotType = "Head" },
                new Item { Name = "Knight's Helm", Category = "Armour", Rarity = "Rare", Price = 400, StatJson = "{\"DEF\": 20, \"END\": 8}", Description = "A noble knight's ceremonial helmet.", ImagePath = "/images/items/knight-helm.png", SlotType = "Head" },
                new Item { Name = "Dragon Scale Helm", Category = "Armour", Rarity = "Epic", Price = 1000, StatJson = "{\"DEF\": 40, \"END\": 15}", Description = "Crafted from genuine dragon scales.", ImagePath = "/images/items/dragon-helm.png", SlotType = "Head" },
                
                // ARMOR - Chest (Common to Legendary)
                new Item { Name = "Cloth Tunic", Category = "Armour", Rarity = "Common", Price = 60, StatJson = "{\"DEF\": 5}", Description = "Simple cloth protection.", ImagePath = "/images/items/cloth-tunic.png", SlotType = "Chest" },
                new Item { Name = "Chainmail", Category = "Armour", Rarity = "Uncommon", Price = 250, StatJson = "{\"DEF\": 15, \"END\": 5}", Description = "Interlocking metal rings provide decent protection.", ImagePath = "/images/items/chainmail.png", SlotType = "Chest" },
                new Item { Name = "Plate Armor", Category = "Armour", Rarity = "Rare", Price = 600, StatJson = "{\"DEF\": 35, \"END\": 12}", Description = "Heavy plate armor for serious warriors.", ImagePath = "/images/items/plate-armor.png", SlotType = "Chest" },
                new Item { Name = "Titan's Breastplate", Category = "Armour", Rarity = "Epic", Price = 1400, StatJson = "{\"DEF\": 60, \"END\": 20, \"STR\": 10}", Description = "Armor fit for a titan.", ImagePath = "/images/items/titan-breastplate.png", SlotType = "Chest" },
                new Item { Name = "Celestial Aegis", Category = "Armour", Rarity = "Legendary", Price = 3500, StatJson = "{\"DEF\": 120, \"END\": 35, \"HP\": 100}", Description = "Divine protection from the heavens.", ImagePath = "/images/items/celestial-aegis.png", SlotType = "Chest" },
                
                // ARMOR - Legs
                new Item { Name = "Leather Pants", Category = "Armour", Rarity = "Common", Price = 45, StatJson = "{\"DEF\": 3}", Description = "Basic leg protection.", ImagePath = "/images/items/leather-pants.png", SlotType = "Legs" },
                new Item { Name = "Steel Greaves", Category = "Armour", Rarity = "Uncommon", Price = 180, StatJson = "{\"DEF\": 12, \"END\": 4}", Description = "Sturdy steel leg armor.", ImagePath = "/images/items/steel-greaves.png", SlotType = "Legs" },
                new Item { Name = "Paladin's Legplates", Category = "Armour", Rarity = "Rare", Price = 500, StatJson = "{\"DEF\": 25, \"END\": 10}", Description = "Holy armor blessed by the light.", ImagePath = "/images/items/paladin-legs.png", SlotType = "Legs" },
                
                // ARMOR - Boots
                new Item { Name = "Worn Boots", Category = "Armour", Rarity = "Common", Price = 30, StatJson = "{\"DEF\": 2}", Description = "Better than barefoot.", ImagePath = "/images/items/worn-boots.png", SlotType = "Boots" },
                new Item { Name = "Swift Boots", Category = "Armour", Rarity = "Uncommon", Price = 160, StatJson = "{\"DEF\": 8, \"AGI\": 5}", Description = "These boots increase your speed.", ImagePath = "/images/items/swift-boots.png", SlotType = "Boots" },
                new Item { Name = "Shadow Step Boots", Category = "Armour", Rarity = "Rare", Price = 450, StatJson = "{\"DEF\": 15, \"AGI\": 12, \"CritChance\": 5}", Description = "Move like a shadow with these enchanted boots.", ImagePath = "/images/items/shadow-boots.png", SlotType = "Boots" },
                
                // ARMOR - Gloves
                new Item { Name = "Leather Gloves", Category = "Armour", Rarity = "Common", Price = 35, StatJson = "{\"DEF\": 2}", Description = "Simple hand protection.", ImagePath = "/images/items/leather-gloves.png", SlotType = "Gloves" },
                new Item { Name = "Gauntlets of Strength", Category = "Armour", Rarity = "Rare", Price = 480, StatJson = "{\"DEF\": 18, \"STR\": 10, \"ATK\": 5}", Description = "These gauntlets enhance your striking power.", ImagePath = "/images/items/strength-gauntlets.png", SlotType = "Gloves" },
                
                // ACCESSORIES
                new Item { Name = "Silver Ring", Category = "Accessory", Rarity = "Uncommon", Price = 120, StatJson = "{\"LCK\": 5}", Description = "A simple ring that brings good fortune.", ImagePath = "/images/items/silver-ring.png", SlotType = "Accessory" },
                new Item { Name = "Amulet of Vitality", Category = "Accessory", Rarity = "Rare", Price = 550, StatJson = "{\"HP\": 50, \"END\": 8}", Description = "Increases your life force.", ImagePath = "/images/items/vitality-amulet.png", SlotType = "Accessory" },
                new Item { Name = "Cloak of Shadows", Category = "Accessory", Rarity = "Epic", Price = 1100, StatJson = "{\"AGI\": 15, \"CritChance\": 15}", Description = "Blend with the shadows.", ImagePath = "/images/items/shadow-cloak.png", SlotType = "Accessory" },
                new Item { Name = "Phoenix Pendant", Category = "Accessory", Rarity = "Legendary", Price = 2800, StatJson = "{\"HP\": 100, \"MP\": 50, \"INT\": 20}", Description = "Contains the essence of a phoenix.", ImagePath = "/images/items/phoenix-pendant.png", SlotType = "Accessory" },
                
                // CONSUMABLES
                new Item { Name = "Health Potion", Category = "Consumable", Rarity = "Common", Price = 25, StatJson = "{\"HealHP\": 50}", Description = "Restores 50 HP.", ImagePath = "/images/items/health-potion.png", SlotType = "" },
                new Item { Name = "Mana Potion", Category = "Consumable", Rarity = "Common", Price = 30, StatJson = "{\"HealMP\": 30}", Description = "Restores 30 MP.", ImagePath = "/images/items/mana-potion.png", SlotType = "" },
                new Item { Name = "Greater Health Potion", Category = "Consumable", Rarity = "Uncommon", Price = 60, StatJson = "{\"HealHP\": 150}", Description = "Restores 150 HP.", ImagePath = "/images/items/greater-health-potion.png", SlotType = "" },
                new Item { Name = "Elixir of Strength", Category = "Consumable", Rarity = "Rare", Price = 100, StatJson = "{\"BuffSTR\": 10, \"Duration\": 300}", Description = "Increases Strength by 10 for 5 minutes.", ImagePath = "/images/items/strength-elixir.png", SlotType = "" },
                new Item { Name = "Scroll of Resurrection", Category = "Consumable", Rarity = "Epic", Price = 500, StatJson = "{\"Revive\": 1}", Description = "Allows you to revive once upon defeat.", ImagePath = "/images/items/resurrection-scroll.png", SlotType = "" },
            };

            await context.Items.AddRangeAsync(items);
        }

        private static async Task SeedQuestsAsync(GameDbContext context)
        {
            var quests = new List<Quest>
            {
                new Quest 
                { 
                    Title = "Lightning Reflexes", 
                    Type = "ClickChallenge", 
                    Description = "Click the target 30 times in 20 seconds!", 
                    RewardGold = 100, 
                    RewardXP = 100, 
                    DailyReset = true,
                    QuestData = "{\"targetClicks\": 30, \"timeLimit\": 20}"
                },
                new Quest 
                { 
                    Title = "Memory Master", 
                    Type = "MemoryMatch", 
                    Description = "Match all pairs in the memory card game.", 
                    RewardGold = 150, 
                    RewardXP = 150, 
                    DailyReset = true,
                    QuestData = "{\"pairs\": 8}"
                },
                new Quest 
                { 
                    Title = "Riddle of the Sphinx", 
                    Type = "Riddle", 
                    Description = "Solve the ancient riddle: 'I speak without a mouth and hear without ears. I have no body, but come alive with wind. What am I?'", 
                    RewardGold = 200, 
                    RewardXP = 200, 
                    DailyReset = false,
                    QuestData = "{\"answer\": \"echo\"}"
                },
                new Quest 
                { 
                    Title = "Puzzle Master", 
                    Type = "SliderPuzzle", 
                    Description = "Solve the ancient slider puzzle.", 
                    RewardGold = 250, 
                    RewardXP = 300, 
                    DailyReset = true,
                    QuestData = "{\"gridSize\": 3}"
                },
                new Quest 
                { 
                    Title = "Daily Login Reward", 
                    Type = "DailyHunt", 
                    Description = "Claim your daily login bonus!", 
                    RewardGold = 50, 
                    RewardXP = 50, 
                    DailyReset = true,
                    QuestData = "{}"
                },
                new Quest 
                { 
                    Title = "Arena Initiation", 
                    Type = "ArenaChallenge", 
                    Description = "Clear Arena Level 1.", 
                    RewardGold = 300, 
                    RewardXP = 500, 
                    DailyReset = false,
                    QuestData = "{\"requiredLevel\": 1}"
                },
                new Quest 
                { 
                    Title = "Boss Hunter", 
                    Type = "ArenaChallenge", 
                    Description = "Defeat the Golem of Ruin (Arena Level 5).", 
                    RewardGold = 1000, 
                    RewardXP = 2000, 
                    DailyReset = false,
                    QuestData = "{\"requiredLevel\": 5}"
                },
                new Quest 
                { 
                    Title = "Collector's Pride", 
                    Type = "Collection", 
                    Description = "Own 3 items of Rare quality or higher.", 
                    RewardGold = 400, 
                    RewardXP = 400, 
                    WeeklyReset = true,
                    QuestData = "{\"rarity\": \"Rare\", \"count\": 3}"
                },
            };

            await context.Quests.AddRangeAsync(quests);
        }

        private static async Task SeedAchievementsAsync(GameDbContext context)
        {
            var achievements = new List<Achievement>
            {
                new Achievement { Code = "FirstBlood", Title = "First Blood", Description = "Win your first Arena battle.", IconPath = "/images/achievements/first-blood.png" },
                new Achievement { Code = "Level5", Title = "Rising Star", Description = "Reach Character Level 5.", IconPath = "/images/achievements/level-5.png" },
                new Achievement { Code = "Level10", Title = "Veteran", Description = "Reach Character Level 10.", IconPath = "/images/achievements/level-10.png" },
                new Achievement { Code = "BossSlayer", Title = "Boss Slayer", Description = "Defeat your first boss enemy.", IconPath = "/images/achievements/boss-slayer.png" },
                new Achievement { Code = "Merchant", Title = "Merchant's Friend", Description = "Make your first shop purchase.", IconPath = "/images/achievements/merchant.png" },
                new Achievement { Code = "Hoarder", Title = "Gold Hoarder", Description = "Accumulate 5,000 gold.", IconPath = "/images/achievements/hoarder.png" },
                new Achievement { Code = "QuestMaster", Title = "Quest Master", Description = "Complete 10 quests.", IconPath = "/images/achievements/quest-master.png" },
                new Achievement { Code = "GoldDigger", Title = "Gold Digger", Description = "Earn 10,000 total gold.", IconPath = "/images/achievements/gold-digger.png" },
                new Achievement { Code = "GearUp", Title = "Fully Equipped", Description = "Equip an item in every equipment slot.", IconPath = "/images/achievements/gear-up.png" },
                new Achievement { Code = "Champion", Title = "Realm Champion", Description = "Clear all 10 Arena levels.", IconPath = "/images/achievements/champion.png" },
            };

            await context.Achievements.AddRangeAsync(achievements);
        }

        private static async Task SeedNewsAsync(GameDbContext context)
        {
            var news = new List<News>
            {
                new News 
                { 
                    Title = "Welcome to Realm of Legends!", 
                    Body = "Embark on your epic journey through the Realm of Legends. Complete quests, defeat enemies in the Arena, and become a legendary hero!",
                    PostedAt = DateTime.UtcNow.AddDays(-2),
                    IsActive = true
                },
                new News 
                { 
                    Title = "Arena Now Open", 
                    Body = "The Arena is now open for all brave warriors! Test your skills against 10 levels of increasingly difficult enemies, including two epic boss encounters.",
                    PostedAt = DateTime.UtcNow.AddDays(-1),
                    IsActive = true
                },
                new News 
                { 
                    Title = "Daily Quests Available", 
                    Body = "Don't forget to check the Quest Board daily for new challenges and rewards. Daily quests reset at midnight UTC!",
                    PostedAt = DateTime.UtcNow,
                    IsActive = true
                },
            };

            await context.News.AddRangeAsync(news);
        }
    }
}
