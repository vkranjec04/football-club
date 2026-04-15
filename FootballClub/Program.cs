using FootballClub.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Registracija mock repozitorija (Singleton - isti objekt kroz cijeli životni vijek aplikacije)
builder.Services.AddSingleton<ClubMockRepository>();
builder.Services.AddSingleton<PlayerMockRepository>();
builder.Services.AddSingleton<MatchMockRepository>();
builder.Services.AddSingleton<CoachMockRepository>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();