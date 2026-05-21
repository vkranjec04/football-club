using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FootballClub.Models.Enums;

namespace FootballClub.Models
{
    public class Match
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        // Foreign keys to Club
        public int HomeClubId { get; set; }
        [ForeignKey(nameof(HomeClubId))]
        public Club HomeClub { get; set; } = null!;

        public int AwayClubId { get; set; }
        [ForeignKey(nameof(AwayClubId))]
        public Club AwayClub { get; set; } = null!;

        // Scores
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

        public int StadiumId { get; set; }
        [ForeignKey(nameof(StadiumId))]
        public Stadium Stadium { get; set; } = null!;
        public MatchStatus Status { get; set; }
        public int Attendance { get; set; }
        public string Referee { get; set; } = string.Empty;
        public string Round { get; set; } = string.Empty;           // npr. "Kolo 5"

        // N-N veza između Player i Match ostvarena kroz PlayerStat
        public List<PlayerStat> PlayerStats { get; set; }

        public Match()
        {
            PlayerStats = new List<PlayerStat>();
        }

        public string Result => $"{HomeScore}:{AwayScore}";
    }
}
