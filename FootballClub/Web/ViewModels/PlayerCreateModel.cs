using System.ComponentModel.DataAnnotations;
using FootballClub.Models.Enums;

namespace FootballClub.Web.ViewModels
{
    public class PlayerCreateModel
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(100)]
        public string Nationality { get; set; } = string.Empty;

        public PlayerPosition Position { get; set; }

        public int JerseyNumber { get; set; }

        [Range(0, 1000)]
        public decimal MarketValue { get; set; }

        public DateTime ContractUntil { get; set; }

        public bool IsInjured { get; set; }

        [Required]
        public int ClubId { get; set; }

        public int? TrainingSessionId { get; set; }
    }
}