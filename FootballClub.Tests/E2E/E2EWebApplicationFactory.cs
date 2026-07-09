using System.Collections.Concurrent;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FootballClub.Tests.E2E;

/// <summary>
/// Variant of <see cref="TestWebApplicationFactory"/> for browser (Playwright) tests. The base
/// factory hosts the app on an in-process TestServer, which a real browser cannot reach, so this
/// one additionally starts the same app on Kestrel bound to a random loopback port. Both hosts
/// share the same in-memory database (EF's in-memory store is process-wide, keyed by name), so
/// the seeded data is identical. The dual-host CreateHost dance is the documented workaround for
/// WebApplicationFactory being hardwired to TestServer (dotnet/aspnetcore#33846, #34702).
/// </summary>
internal sealed class E2EWebApplicationFactory : TestWebApplicationFactory
{
    private IHost? _kestrelHost;
    private int _hostCount;

    public E2EWebApplicationFactory(string databaseName)
        : base(databaseName)
    {
    }

    // Each host (TestServer replay and Kestrel replay) gets its own in-memory database. The two
    // Program replays run on separate threads and would otherwise seed one shared store
    // concurrently, corrupting it with duplicate rows. The browser only ever talks to the
    // Kestrel host, so the stores diverging is harmless.
    internal override string ResolveDatabaseName()
        => $"{base.ResolveDatabaseName()}-host{Interlocked.Increment(ref _hostCount)}";

    /// <summary>Root URL of the Kestrel host, e.g. "http://127.0.0.1:53123/".</summary>
    public Uri ServerAddress
    {
        get
        {
            EnsureServer();
            return ClientOptions.BaseAddress;
        }
    }

    /// <summary>Warning+ log entries from the server, for diagnosing E2E failures.</summary>
    public ConcurrentQueue<string> ServerLogs { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);

        // The base factory points the webroot at an empty temp folder (fine for API tests, where
        // static files never load). A browser needs the real css/js, so point back at the app's
        // wwwroot. Last UseWebRoot call wins.
        builder.UseWebRoot(FindRealWebRoot());

        builder.ConfigureLogging(logging => logging.AddProvider(new CollectingLoggerProvider(ServerLogs)));
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        // First build: the TestServer host WebApplicationFactory insists on managing.
        var testHost = builder.Build();

        // Second build of the same deferred builder: identical app, but served by Kestrel on a
        // random free port so Playwright's browser can connect. It must be started before the
        // test host for the bound address to be observable.
        builder.ConfigureWebHost(webHostBuilder =>
            webHostBuilder.UseKestrel(options => options.Listen(IPAddress.Loopback, 0)));
        _kestrelHost = builder.Build();
        _kestrelHost.Start();

        var addresses = _kestrelHost.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>();
        ClientOptions.BaseAddress = addresses!.Addresses.Select(address => new Uri(address)).First();

        testHost.Start();
        return testHost;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _kestrelHost?.Dispose();
        }

        base.Dispose(disposing);
    }

    private void EnsureServer()
    {
        // Touching a client forces the lazy WebApplicationFactory bootstrap, which runs
        // CreateHost above and fills in ClientOptions.BaseAddress.
        using var _ = CreateDefaultClient();
    }

    private static string FindRealWebRoot()
    {
        // Tests run from FootballClub.Tests/bin/<config>/net8.0; walk up to the repo root and
        // locate the app project's wwwroot.
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, "FootballClub", "wwwroot");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Could not locate FootballClub/wwwroot above " + AppContext.BaseDirectory);
    }

    private sealed class CollectingLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentQueue<string> _sink;

        public CollectingLoggerProvider(ConcurrentQueue<string> sink) => _sink = sink;

        public ILogger CreateLogger(string categoryName) => new CollectingLogger(categoryName, _sink);

        public void Dispose()
        {
        }

        private sealed class CollectingLogger : ILogger
        {
            private readonly string _category;
            private readonly ConcurrentQueue<string> _sink;

            public CollectingLogger(string category, ConcurrentQueue<string> sink)
            {
                _category = category;
                _sink = sink;
            }

            public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

            public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                {
                    return;
                }

                _sink.Enqueue($"[{logLevel}] {_category}: {formatter(state, exception!)}{(exception is null ? string.Empty : $"\n{exception}")}");
            }
        }
    }
}
