namespace FootballClub.Models
{
    // Ova klasa implementira N-N vezu između Player i Match.
    // Jedan igrač igra u više utakmica, jedna utakmica ima više igrača.
    public class PlayerStat
    {
        public int Id { get; set; }

        // N strana od Player 1-N Stats
        public Player Player { get; set; }

        // N strana od Match 1-N PlayerStats
        public Match Match { get; set; }

        public int Goals { get; set; }
        public int Assists { get; set; }
        public int MinutesPlayed { get; set; }
        public int YellowCards { get; set; }
        public bool RedCard { get; set; }
        public double Rating { get; set; }          // 1.0 - 10.0
    }
}
