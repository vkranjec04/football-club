using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FootballClub.Models
{
    public class Club
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        public int FoundedYear { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Budget { get; set; }          // u milijunima EUR

        [Required]
        [MaxLength(50)]
        public string LeagueName { get; set; } = string.Empty;

        public int HomeStadiumId { get; set; }
        public virtual Stadium HomeStadium { get; set; } = null!;
        
        // Allow multiple staff members (coaches, physios, assistants)
        public virtual ICollection<Staff> StaffMembers { get; set; }

        // 1-N: jedan klub ima više igrača
        public virtual ICollection<Player> Players { get; set; }

        // 1-N: jedan klub ima više domaćih i gostujućih utakmica
        public virtual ICollection<Match> HomeMatches { get; set; }
        
        public virtual ICollection<Match> AwayMatches { get; set; }

        public Club()
        {
            Players = new HashSet<Player>();
            HomeMatches = new HashSet<Match>();
            AwayMatches = new HashSet<Match>();
            StaffMembers = new HashSet<Staff>();
        }
    }
}
