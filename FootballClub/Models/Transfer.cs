namespace FootballClub.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; } = null!;
        public int FromClubId { get; set; }
        public Club FromClub { get; set; } = null!;
        public int ToClubId { get; set; }
        public Club ToClub { get; set; } = null!;
        public DateTime TransferDate { get; set; }
        public decimal Fee { get; set; }            // u milijunima EUR
    }
}
