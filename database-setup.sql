IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [Achievements] (
        [AchievementId] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Title] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [IconPath] nvarchar(200) NOT NULL,
        CONSTRAINT [PK_Achievements] PRIMARY KEY ([AchievementId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [EmailConfirmedAt] datetime2 NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [Items] (
        [ItemId] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [Category] nvarchar(50) NOT NULL,
        [Rarity] nvarchar(20) NOT NULL,
        [Price] int NOT NULL,
        [StatJson] nvarchar(max) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [ImagePath] nvarchar(200) NOT NULL,
        [SlotType] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_Items] PRIMARY KEY ([ItemId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [News] (
        [NewsId] int NOT NULL IDENTITY,
        [Title] nvarchar(200) NOT NULL,
        [Body] nvarchar(max) NOT NULL,
        [PostedAt] datetime2 NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_News] PRIMARY KEY ([NewsId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [Players] (
        [PlayerId] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [CharacterName] nvarchar(50) NOT NULL,
        [Class] nvarchar(20) NOT NULL,
        [Level] int NOT NULL,
        [XP] int NOT NULL,
        [Gold] int NOT NULL,
        [HP] int NOT NULL,
        [MP] int NOT NULL,
        [Strength] int NOT NULL,
        [Agility] int NOT NULL,
        [Intelligence] int NOT NULL,
        [Endurance] int NOT NULL,
        [Luck] int NOT NULL,
        [MaxHP] int NOT NULL,
        [MaxMP] int NOT NULL,
        [AttackPower] int NOT NULL,
        [DefenseRating] int NOT NULL,
        [CriticalHitChance] float NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [LastLoginAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([PlayerId])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [Quests] (
        [QuestId] int NOT NULL IDENTITY,
        [Title] nvarchar(100) NOT NULL,
        [Type] nvarchar(50) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [RewardGold] int NOT NULL,
        [RewardXP] int NOT NULL,
        [RewardItemId] int NULL,
        [DailyReset] bit NOT NULL,
        [WeeklyReset] bit NOT NULL,
        [QuestData] nvarchar(max) NULL,
        CONSTRAINT [PK_Quests] PRIMARY KEY ([QuestId]),
        CONSTRAINT [FK_Quests_Items_RewardItemId] FOREIGN KEY ([RewardItemId]) REFERENCES [Items] ([ItemId]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [ArenaProgresses] (
        [ArenaProgressId] int NOT NULL IDENTITY,
        [PlayerId] int NOT NULL,
        [CurrentLevel] int NOT NULL,
        [HighestLevel] int NOT NULL,
        [TotalWins] int NOT NULL,
        [TotalLosses] int NOT NULL,
        [LastFightAt] datetime2 NULL,
        CONSTRAINT [PK_ArenaProgresses] PRIMARY KEY ([ArenaProgressId]),
        CONSTRAINT [FK_ArenaProgresses_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [FightLogs] (
        [FightLogId] int NOT NULL IDENTITY,
        [PlayerId] int NOT NULL,
        [Level] int NOT NULL,
        [Outcome] nvarchar(20) NOT NULL,
        [XPEarned] int NOT NULL,
        [GoldEarned] int NOT NULL,
        [DroppedItemId] int NULL,
        [FoughtAt] datetime2 NOT NULL,
        CONSTRAINT [PK_FightLogs] PRIMARY KEY ([FightLogId]),
        CONSTRAINT [FK_FightLogs_Items_DroppedItemId] FOREIGN KEY ([DroppedItemId]) REFERENCES [Items] ([ItemId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_FightLogs_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [Inventories] (
        [InventoryId] int NOT NULL IDENTITY,
        [PlayerId] int NOT NULL,
        [ItemId] int NOT NULL,
        [Quantity] int NOT NULL,
        [IsEquipped] bit NOT NULL,
        [SlotType] nvarchar(20) NULL,
        [AcquiredAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Inventories] PRIMARY KEY ([InventoryId]),
        CONSTRAINT [FK_Inventories_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([ItemId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Inventories_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [PlayerAchievements] (
        [PlayerAchievementId] int NOT NULL IDENTITY,
        [PlayerId] int NOT NULL,
        [AchievementId] int NOT NULL,
        [UnlockedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PlayerAchievements] PRIMARY KEY ([PlayerAchievementId]),
        CONSTRAINT [FK_PlayerAchievements_Achievements_AchievementId] FOREIGN KEY ([AchievementId]) REFERENCES [Achievements] ([AchievementId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PlayerAchievements_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [PurchaseHistories] (
        [PurchaseId] int NOT NULL IDENTITY,
        [PlayerId] int NOT NULL,
        [ItemId] int NOT NULL,
        [GoldSpent] int NOT NULL,
        [PurchasedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PurchaseHistories] PRIMARY KEY ([PurchaseId]),
        CONSTRAINT [FK_PurchaseHistories_Items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [Items] ([ItemId]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PurchaseHistories_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE TABLE [PlayerQuests] (
        [PlayerQuestId] int NOT NULL IDENTITY,
        [PlayerId] int NOT NULL,
        [QuestId] int NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [CompletedAt] datetime2 NULL,
        [ExpiresAt] datetime2 NULL,
        [AssignedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_PlayerQuests] PRIMARY KEY ([PlayerQuestId]),
        CONSTRAINT [FK_PlayerQuests_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Players] ([PlayerId]) ON DELETE CASCADE,
        CONSTRAINT [FK_PlayerQuests_Quests_QuestId] FOREIGN KEY ([QuestId]) REFERENCES [Quests] ([QuestId]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Achievements_Code] ON [Achievements] ([Code]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_ArenaProgresses_PlayerId] ON [ArenaProgresses] ([PlayerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FightLogs_DroppedItemId] ON [FightLogs] ([DroppedItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_FightLogs_PlayerId] ON [FightLogs] ([PlayerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Inventories_ItemId] ON [Inventories] ([ItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Inventories_PlayerId] ON [Inventories] ([PlayerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PlayerAchievements_AchievementId] ON [PlayerAchievements] ([AchievementId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_PlayerAchievements_PlayerId_AchievementId] ON [PlayerAchievements] ([PlayerId], [AchievementId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PlayerQuests_PlayerId] ON [PlayerQuests] ([PlayerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PlayerQuests_QuestId] ON [PlayerQuests] ([QuestId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Players_UserId] ON [Players] ([UserId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseHistories_ItemId] ON [PurchaseHistories] ([ItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_PurchaseHistories_PlayerId] ON [PurchaseHistories] ([PlayerId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Quests_RewardItemId] ON [Quests] ([RewardItemId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260606192245_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260606192245_InitialCreate', N'8.0.0');
END;
GO

COMMIT;
GO

