using FootballClub.Models.Enums;

namespace FootballClub.Models;

public class PlayerScheduleItem
{
    public int Id { get; set; }
    public int PlayerId { get; set; }
    public virtual Player Player { get; set; } = new();
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public ScheduleResponsibilityType ResponsibilityType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AssignedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public TimeSpan Duration => EndTime - StartTime;
}
