using FootballClub.Data;
using FootballClub.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure for Razor Pages + MVC Controllers (if you use both)
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// Register mock repositories (Scoped if they use DbContext, Singleton if static-only)
builder.Services.AddScoped<ClubMockRepository>();
builder.Services.AddScoped<PlayerMockRepository>();
builder.Services.AddScoped<MatchMockRepository>();
builder.Services.AddScoped<CoachMockRepository>();
builder.Services.AddScoped<TrainingMockRepository>();
builder.Services.AddScoped<PlayerScheduleMockRepository>();

var app = builder.Build();

// Ensure database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    DataSeeder.SeedDatabase(context);
}

app.UseStaticFiles();
app.UseRouting();

// Map Razor Pages first (they have priority)
app.MapRazorPages();

// Map MVC controllers (fallback)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();