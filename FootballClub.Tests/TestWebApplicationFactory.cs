using FootballClub.Data;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FootballClub.Tests;

internal class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName;
    private readonly string _webRootPath;

    /// <summary>Deterministic AI client used by the AI endpoint tests; configure it per test.</summary>
    internal FakeAiClient AiClient { get; } = new();

    /// <summary>
    /// Name of the in-memory database for the host being built. EF's in-memory store is shared
    /// process-wide by name, so a factory that builds more than one host from the same Program
    /// (see E2EWebApplicationFactory) must return a distinct name per host — otherwise both
    /// hosts race to seed the same store and duplicate the seed data.
    /// </summary>
    internal virtual string ResolveDatabaseName() => _databaseName;

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

            // Resolve the name here, not inside the options lambda: this outer callback runs once
            // per host build, while the options lambda runs per scope (DbContextOptions is
            // scoped) and would mint a fresh empty store on every request.
            var databaseName = ResolveDatabaseName();
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));

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
