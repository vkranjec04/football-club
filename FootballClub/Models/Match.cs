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
        public Club HomeClub { get; set; }

        public int AwayClubId { get; set; }
        [ForeignKey(nameof(AwayClubId))]
        public Club AwayClub { get; set; }

        // Scores
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
