using FootballClub.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballClub.Models;

public class TrainingSession
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public virtual Club Club { get; set; } = null!;
    public string Title { get; set; } = string.Empty;
    public string FocusArea { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public TrainingIntensity Intensity { get; set; }
    [Column("LeadCoachId")]
    public int? LeadStaffId { get; set; }
    public virtual Staff? LeadStaff { get; set; }
    public List<Player> Participants { get; set; } = new();
    public string Notes { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }

    public bool IsToday => StartTime.Date == DateTime.Today;
    public bool IsUpcoming => StartTime >= DateTime.Now;
}
