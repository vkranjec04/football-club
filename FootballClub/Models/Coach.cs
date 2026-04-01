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

        // N strana od Club 1-1 Coach
        public Club Club { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
