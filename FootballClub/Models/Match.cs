using FootballClub.Models.Enums;

namespace FootballClub.Models
{
    public class Match
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public Club HomeClub { get; set; }
        public Club AwayClub { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public Stadium Stadium { get; set; }
        public MatchStatus Status { get; set; }
        public int Attendance { get; set; }
        public string Referee { get; set; }
        public string Round { get; set; }           // npr. "Kolo 5"

        // N-N veza između Player i Match ostvarena kroz PlayerStat
        public List<PlayerStat> PlayerStats { get; set; }

        public Match()
        {
            PlayerStats = new List<PlayerStat>();
        }

        public string Result => $"{HomeScore}:{AwayScore}";
    }
}
