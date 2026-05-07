using FootballClub.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Stadium> Stadiums { get; set; }
        public DbSet<PlayerStat> PlayerStats { get; set; }
        public DbSet<Transfer> Transfers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Many-to-Many or specific cascading rules here
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeClub)
                .WithMany(c => c.HomeMatches)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayClub)
                .WithMany(c => c.AwayMatches)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.FromClub)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.ToClub)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Coach>()
                .HasOne(c => c.Club)
                .WithOne(c => c.Coach)
                .HasForeignKey<Coach>(c => c.ClubId);

                // Configure PlayerStat relationships to avoid circular cascade deletes
                modelBuilder.Entity<PlayerStat>()
                    .HasOne(ps => ps.Player)
                    .WithMany(p => p.Stats)
                    .OnDelete(DeleteBehavior.NoAction);

                modelBuilder.Entity<PlayerStat>()
                    .HasOne(ps => ps.Match)
                    .WithMany(m => m.PlayerStats)
                    .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
