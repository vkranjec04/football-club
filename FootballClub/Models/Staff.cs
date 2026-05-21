using System.ComponentModel.DataAnnotations.Schema;

namespace FootballClub.Models
{
    [Table("Coaches")]
    public class Staff
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public DateTime ContractUntil { get; set; }
        public string Role { get; set; } = "Head Coach";
        public bool IsDeleted { get; set; }

        public int ClubId { get; set; }
        public virtual Club Club { get; set; } = null!;

        public string FullName => $"{FirstName} {LastName}";
    }
}