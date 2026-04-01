using System.Security.Cryptography.Xml;
using FootballClub.Models.Enums;

namespace FootballClub.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public PlayerPosition Position { get; set; }
        public int JerseyNumber { get; set; }
        public decimal MarketValue { get; set; }     // u milijunima EUR
        public DateTime ContractUntil { get; set; }
        public bool IsInjured { get; set; }

        // N strana od Club 1-N Players
        public Club Club { get; set; }

        // 1-N: jedan igrač ima više statistika (po utakmici)
        public List<PlayerStat> Stats { get; set; }

        // 1-N: jedan igrač može imati više transfera
        public List<Transfer> Transfers { get; set; }

        public Player()
        {
            Stats = new List<PlayerStat>();
            Transfers = new List<Transfer>();
        }

        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
    }
}
