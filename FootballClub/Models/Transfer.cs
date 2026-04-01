namespace FootballClub.Models
{
    public class Transfer
    {
        public int Id { get; set; }
        public Player Player { get; set; }
        public Club FromClub { get; set; }
        public Club ToClub { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal Fee { get; set; }            // u milijunima EUR
    }
}
