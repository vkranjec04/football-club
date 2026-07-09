using FootballClub.Data;
using FootballClub.Models;

namespace FootballClub.Web.Features.ActivityLog;

/// <summary>
/// Persists audit entries to the database. Resolves the acting user from the current
/// <see cref="HttpContext"/> and writes through a short-lived scope of its own, so the audit
/// write never participates in — nor is rolled back by — the request's unit of work. Failures
/// are swallowed and logged: auditing must not break the operation being audited.
/// </summary>
public sealed class DbActivityLogger : IActivityLogger
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DbActivityLogger> _logger;

    public DbActivityLogger(
        IServiceScopeFactory scopeFactory,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DbActivityLogger> logger)
    {
        _scopeFactory = scopeFactory;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogAsync(
        string action,
        string? entityType = null,
        string? entityId = null,
        string? description = null,
        bool success = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var entry = new FootballClub.Models.ActivityLog
            {
                TimestampUtc = DateTime.UtcNow,
                UserName = ResolveUserName(httpContext),
                Action = Truncate(action, 100) ?? string.Empty,
                EntityType = Truncate(entityType, 100),
                EntityId = Truncate(entityId, 100),
                Description = Truncate(description, 1000),
                IpAddress = Truncate(httpContext?.Connection.RemoteIpAddress?.ToString(), 64),
                Success = success
            };

            // A dedicated scope (and therefore a dedicated DbContext) keeps the audit write
            // isolated from whatever the request's own DbContext is doing.
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.ActivityLogs.Add(entry);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write activity log entry for action {Action}.", action);
        }
    }

    private static string ResolveUserName(HttpContext? httpContext)
    {
        var name = httpContext?.User.Identity?.IsAuthenticated == true
            ? httpContext.User.Identity?.Name
            : null;
        return string.IsNullOrWhiteSpace(name) ? "anonymous" : name;
    }

    private static string? Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        return value.Length <= maxLength ? value : value[..maxLength];
    }
}
