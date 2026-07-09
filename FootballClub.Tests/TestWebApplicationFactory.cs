using FootballClub.Data;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FootballClub.Tests;

internal sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName;
    private readonly string _webRootPath;

    /// <summary>Deterministic AI client used by the AI endpoint tests; configure it per test.</summary>
    internal FakeAiClient AiClient { get; } = new();

    public TestWebApplicationFactory(string databaseName)
    {
        _databaseName = databaseName;
        _webRootPath = Path.Combine(Path.GetTempPath(), "footballclub-tests", _databaseName);
        Directory.CreateDirectory(_webRootPath);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.UseWebRoot(_webRootPath);
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = TestJwtTokenGenerator.JwtKey,
                ["Jwt:Issuer"] = TestJwtTokenGenerator.JwtIssuer,
                ["Jwt:Audience"] = TestJwtTokenGenerator.JwtAudience,
                ["Authentication:Google:ClientId"] = "test-client-id",
                ["Authentication:Google:ClientSecret"] = "test-client-secret",
                ["Uploads:RootPath"] = "wwwroot/uploads",
                ["Uploads:MaxFileSizeBytes"] = "10485760"
            };

            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(_databaseName));

            // Swap the real AI provider for a deterministic fake the tests control.
            foreach (var aiDescriptor in services.Where(d => d.ServiceType == typeof(IAiClient)).ToList())
            {
                services.Remove(aiDescriptor);
            }

            services.AddSingleton<IAiClient>(AiClient);

            // Bypass antiforgery so tests can POST to the MVC extraction endpoints without a token.
            services.AddSingleton<IAntiforgery, NoOpAntiforgery>();
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && Directory.Exists(_webRootPath))
        {
            Directory.Delete(_webRootPath, true);
        }
    }
}
