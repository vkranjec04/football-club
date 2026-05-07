namespace FootballClub.Models
{
    public class Coach
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime ContractUntil { get; set; }
        public string Role { get; set; } = "Head Coach";

        public int ClubId { get; set; }  // Foreign key property
        public virtual Club Club { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
