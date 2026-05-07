using FootballClub.Models.Enums;

namespace FootballClub.Models;

public class TrainingSession
{
    public int Id { get; set; }
    public Club Club { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string FocusArea { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public TrainingIntensity Intensity { get; set; }
    public Coach? LeadCoach { get; set; }
    public List<Player> Participants { get; set; } = new();
    public string Notes { get; set; } = string.Empty;

    public bool IsToday => StartTime.Date == DateTime.Today;
    public bool IsUpcoming => StartTime >= DateTime.Now;
}
