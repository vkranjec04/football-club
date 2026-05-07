using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FootballClub.Models.Enums;

namespace FootballClub.Models
{
    public class Player
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        [MaxLength(100)]
        public string Nationality { get; set; }

        public PlayerPosition Position { get; set; }
        
        public int JerseyNumber { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MarketValue { get; set; }     // u milijunima EUR
        
        public DateTime ContractUntil { get; set; }
        
        public bool IsInjured { get; set; }

        // N strana od Club 1-N Players
        public virtual Club Club { get; set; }

        // 1-N: jedan igrač ima više statistika (po utakmici)
        public virtual ICollection<PlayerStat> Stats { get; set; }

        // 1-N: jedan igrač može imati više transfera
        public virtual ICollection<Transfer> Transfers { get; set; }

        public Player()
        {
            Stats = new HashSet<PlayerStat>();
            Transfers = new HashSet<Transfer>();
        }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
        
        [NotMapped]
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
    }
}
