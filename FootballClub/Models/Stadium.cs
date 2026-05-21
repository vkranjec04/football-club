namespace FootballClub.Models
{
    public class Stadium
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int YearBuilt { get; set; }

        public Stadium()
        {
        }
    }
}
