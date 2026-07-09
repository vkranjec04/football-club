namespace FootballClub.Web.Features.ActivityLog;

/// <summary>
/// Records audit-trail entries (who did what, when, from where). The current user and the
/// caller's IP are resolved by the implementation, so callers only describe <em>what</em>
/// happened. Implemented by <see cref="DbActivityLogger"/>. Mirrors the IFileStorage /
/// IAiClient abstraction so the sink (the database today, a file or external service later)
/// stays swappable without touching callers.
/// </summary>
public interface IActivityLogger
{
    /// <summary>
    /// Writes a single audit entry. Implementations must be resilient: a logging failure
    /// must never propagate into — and break — the operation being audited.
    /// </summary>
    /// <param name="action">What happened, e.g. "Login", "Create", "Delete".</param>
    /// <param name="entityType">The kind of thing acted on, e.g. "Player" (optional).</param>
    /// <param name="entityId">Identifier of the affected entity (optional).</param>
    /// <param name="description">Human-readable detail (optional).</param>
    /// <param name="success">False when the audited action failed.</param>
    Task LogAsync(
        string action,
        string? entityType = null,
        string? entityId = null,
        string? description = null,
        bool success = true,
        CancellationToken cancellationToken = default);
}
