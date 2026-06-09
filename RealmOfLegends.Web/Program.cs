using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using RealmOfLegends.Data.Context;
using RealmOfLegends.Core.Entities;
using RealmOfLegends.Data.Seeders;
using RealmOfLegends.Web.Filters;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to accept any host header (required for ngrok and other tunneling services)
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AllowSynchronousIO = true;
});

// Add DbContext with SQL Server (use LocalDB / configured connection string)
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("RealmOfLegends.Data")));

// Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    
    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Disabled for development
})
.AddEntityFrameworkStores<GameDbContext>()
.AddDefaultTokenProviders();

// Configure cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    // Allow HTTP for ngrok tunneling (ngrok provides HTTPS, but some requests may come as HTTP internally)
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
        ? CookieSecurePolicy.SameAsRequest 
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax; // Changed from Strict to Lax for better ngrok compatibility
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// Register Arena Service
builder.Services.AddScoped<RealmOfLegends.Core.Services.Arena.IArenaService, RealmOfLegends.Data.Services.ArenaService>();

var app = builder.Build();

// Allow forwarded headers for ngrok and reverse proxies
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor 
        | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error/500");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Handle status code errors
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages so the app can serve pages in the browser
app.MapRazorPages();

// Database initialization
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    // Attempt to apply migrations on startup (safe no-op if already applied)
    try
    {
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
    }
    catch (Exception migEx)
    {
        // Log but continue; migration may fail if DB not ready yet
        logger.LogWarning(migEx, "Database migration attempt failed on startup.");
    }

    // Seed data if needed
    try
    {
        await DatabaseSeeder.SeedAsync(context);
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception seedEx)
    {
        logger.LogWarning(seedEx, "Database seeding failed on startup.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while initializing the database.");
    // Continue running the app even if database initialization fails
}

app.Run();
