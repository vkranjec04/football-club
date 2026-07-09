using FootballClub.Data;
using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with retry logic for transient failures. Retry is generous because the
// production database (Azure SQL serverless free tier) auto-pauses and takes ~30-60s to
// resume on a cold start; a short retry window would time out before the DB wakes.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// Configure for Razor Pages + MVC Controllers (if you use both)
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews(options =>
{
    // Auto-audit every state-changing request (MVC form posts and API calls alike).
    options.Filters.Add<ActivityLogActionFilter>();
});
builder.Services.AddScoped<IUserService, UserService>();

// Audit logging: record who-did-what to the database. The logger resolves the acting user
// from the current request, so it needs the HttpContext accessor. Registered once behind
// an interface so the sink is swappable.
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IActivityLogger, DbActivityLogger>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// AI client: use Google Gemini when an API key is configured, otherwise a no-op
// fallback so the app and tests run without a key.
builder.Services.Configure<AiOptions>(builder.Configuration.GetSection("Ai"));
if (!string.IsNullOrWhiteSpace(builder.Configuration["Ai:ApiKey"]))
{
    builder.Services.AddHttpClient<IAiClient, GeminiAiClient>();
}
else
{
    builder.Services.AddScoped<IAiClient, NullAiClient>();
}

builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = builder.Environment.IsEnvironment("Test")
    ? "test-jwt-key-please-change-32-chars-min"
    : jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
var jwtIssuer = builder.Environment.IsEnvironment("Test")
    ? "FootballClub.Tests"
    : jwtSection["Issuer"];
var jwtAudience = builder.Environment.IsEnvironment("Test")
    ? "FootballClub.Tests"
    : jwtSection["Audience"];
var googleClientId = builder.Configuration["Authentication:Google:ClientId"];
var googleClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
if (string.IsNullOrWhiteSpace(googleClientId) || string.IsNullOrWhiteSpace(googleClientSecret))
{
    if (!builder.Environment.IsEnvironment("Test"))
    {
        throw new InvalidOperationException("Authentication:Google:ClientId is missing.");
    }

    googleClientId = "test-client-id";
    googleClientSecret = "test-client-secret";
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = System.Security.Claims.ClaimTypes.Name,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    })
    .AddCookie("External", options =>
    {
        options.Cookie.Name = ".FootballClub.External";
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    })
    .AddGoogle(options =>
    {
        options.SignInScheme = "External";
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
        options.SaveTokens = true;
        options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
        options.CorrelationCookie.SameSite = SameSiteMode.None;
    });

// MCP server: exposes the club's domain (players, clubs, matches, ...) as tools an agentic
// IDE (Claude Code, VS Code, Cursor) can call directly over the Model Context Protocol at
// "/mcp". Deliberately unauthenticated - same trust level as the app's anonymous GET
// endpoints - since it is a local development surface, not a public API.
builder.Services.AddMcpServer()
    .WithHttpTransport()
    .WithToolsFromAssembly();

builder.Services.AddAuthorization();

// Register mock repositories (Scoped if they use DbContext, Singleton if static-only)
builder.Services.AddScoped<ClubMockRepository>();
builder.Services.AddScoped<PlayerMockRepository>();
builder.Services.AddScoped<MatchMockRepository>();
builder.Services.AddScoped<StaffMockRepository>();
builder.Services.AddScoped<TrainingMockRepository>();
builder.Services.AddScoped<PlayerScheduleMockRepository>();

var supportedCultures = new[]
{
    new CultureInfo("hr"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("hr");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Apply schema and seed data once at startup. Schema creation/migration is fatal
// (the app cannot run without it), but seeding is best-effort: a seed failure must
// not take down an otherwise healthy app — especially on free-tier cold starts.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    if (app.Environment.IsEnvironment("Test"))
    {
        await context.Database.EnsureCreatedAsync();
    }
    else
    {
        await context.Database.MigrateAsync();
    }

    try
    {
        // Demo data is gated so production can opt out (Seed:DemoData=false) while still
        // seeding identity (admin/user). Both seeders are idempotent on re-run.
        if (app.Configuration.GetValue("Seed:DemoData", true))
        {
            DataSeeder.SeedDatabase(context);
        }

        await DataSeeder.SeedIdentityDataAsync(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Database seeding failed; continuing startup without seed data.");
    }
}

app.UseStaticFiles();

if (!app.Environment.IsEnvironment("Test"))
{
    app.UseHttpsRedirection();
}

app.UseRequestLocalization(app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value);

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Map Razor Pages first (they have priority)
app.MapRazorPages();

// Map MVC controllers (fallback)
app.MapControllers();
app.MapMcp("/mcp");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public partial class Program { }
