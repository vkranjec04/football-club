namespace FootballClub.Web.Features.ActivityLog;

public class ActivityLogDto
{
    public int Id { get; set; }

    public DateTime TimestampUtc { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string? EntityType { get; set; }

    public string? EntityId { get; set; }

    public string? Description { get; set; }

    public string? IpAddress { get; set; }

    public bool Success { get; set; }
}
