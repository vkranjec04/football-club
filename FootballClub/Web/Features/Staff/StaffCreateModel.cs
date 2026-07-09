using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Features.Staff
{
    public class StaffCreateModel
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Nationality { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public DateTime ContractUntil { get; set; }

        [Required]
        [MaxLength(100)]
        public string Role { get; set; } = "Head Coach";

        [Required]
        public int ClubId { get; set; }
    }
}
