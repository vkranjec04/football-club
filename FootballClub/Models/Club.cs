using System.Numerics;
using System.Text.RegularExpressions;

namespace FootballClub.Models
{
    public class Club
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public int FoundedYear { get; set; }
        public decimal Budget { get; set; }          // u milijunima EUR
        public string LeagueName { get; set; }
        public Stadium HomeStadium { get; set; }
        public Coach? Coach { get; set; }

        // 1-N: jedan klub ima više igrača
        public List<Player> Players { get; set; }

        // 1-N: jedan klub ima više domaćih i gostujućih utakmica
        public List<Match> HomeMatches { get; set; }
        public List<Match> AwayMatches { get; set; }

        public Club()
        {
            Players = new List<Player>();
            HomeMatches = new List<Match>();
            AwayMatches = new List<Match>();
        }
    }
}
