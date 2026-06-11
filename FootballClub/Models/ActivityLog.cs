using System.ComponentModel.DataAnnotations;

namespace FootballClub.Models;

/// <summary>
/// An audit-trail entry recording a single significant action (a login, or a
/// create/update/delete) together with who performed it and from where. Written by
/// <c>DbActivityLogger</c> (see <c>FootballClub.Web.Services.IActivityLogger</c>) and
/// surfaced through the admin-only activity-log viewer.
/// </summary>
public class ActivityLog
{
    [Key]
    public int Id { get; set; }

    /// <summary>When the action happened (UTC).</summary>
    public DateTime TimestampUtc { get; set; }

    /// <summary>The authenticated user who performed the action, or "anonymous".</summary>
    [Required]
    [MaxLength(256)]
    public string UserName { get; set; } = "anonymous";

    /// <summary>What happened, e.g. "Login", "LoginFailed", "Create", "Edit", "Delete".</summary>
    [Required]
    [MaxLength(100)]
    public string Action { get; set; } = string.Empty;

    /// <summary>The kind of thing acted on, e.g. "Player", "Staff", "Auth" (optional).</summary>
    [MaxLength(100)]
    public string? EntityType { get; set; }

    /// <summary>Identifier of the affected entity, kept as text to allow non-numeric keys (optional).</summary>
    [MaxLength(100)]
    public string? EntityId { get; set; }

    /// <summary>Human-readable detail, e.g. "POST /Player/Create -> 302" (optional).</summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>The caller's IP address, when available.</summary>
    [MaxLength(64)]
    public string? IpAddress { get; set; }

    /// <summary>False when the action failed (validation error, bad credentials, 4xx/5xx, exception).</summary>
    public bool Success { get; set; } = true;
}
