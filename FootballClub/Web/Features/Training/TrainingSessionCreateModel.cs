using System.ComponentModel.DataAnnotations;
using FootballClub.Models.Enums;

namespace FootballClub.Web.Features.Training
{
    public class TrainingSessionCreateModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string FocusArea { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [MaxLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public TrainingIntensity Intensity { get; set; }

        public int? LeadStaffId { get; set; }

        [Required]
        public int ClubId { get; set; }

        [MaxLength(1000)]
        public string Notes { get; set; } = string.Empty;
    }
}
