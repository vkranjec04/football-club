using FootballClub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Staff> StaffMembers { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Stadium> Stadiums { get; set; }
        public DbSet<PlayerStat> PlayerStats { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<TrainingSession> TrainingSessions { get; set; }
        public DbSet<PlayerScheduleItem> PlayerScheduleItems { get; set; }
        public DbSet<LeagueStanding> LeagueStandings { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Many-to-Many or specific cascading rules here
            modelBuilder.Entity<Match>()
                .HasOne(m => m.HomeClub)
                .WithMany(c => c.HomeMatches)
                .HasForeignKey(m => m.HomeClubId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Match>()
                .HasOne(m => m.AwayClub)
                .WithMany(c => c.AwayMatches)
                .HasForeignKey(m => m.AwayClubId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.FromClub)
                .WithMany()
                .HasForeignKey(t => t.FromClubId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.ToClub)
                .WithMany()
                .HasForeignKey(t => t.ToClubId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transfer>()
                .HasOne(t => t.Player)
                .WithMany(p => p.Transfers)
                .HasForeignKey(t => t.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Transfer>()
                .Property(t => t.Fee)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Staff>()
                .HasOne(c => c.Club)
                .WithMany(c => c.StaffMembers)
                .HasForeignKey(c => c.ClubId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TrainingSession>()
                .HasOne(ts => ts.LeadStaff)
                .WithMany()
                .HasForeignKey(ts => ts.LeadStaffId)
                .HasConstraintName("FK_TrainingSessions_Coaches_LeadCoachId")
                .OnDelete(DeleteBehavior.NoAction);

                // Configure PlayerStat relationships to avoid circular cascade deletes
                modelBuilder.Entity<PlayerStat>()
                    .HasOne(ps => ps.Player)
                    .WithMany(p => p.Stats)
                    .HasForeignKey(ps => ps.PlayerId)
                    .OnDelete(DeleteBehavior.NoAction);

                modelBuilder.Entity<PlayerStat>()
                    .HasOne(ps => ps.Match)
                    .WithMany(m => m.PlayerStats)
                    .HasForeignKey(ps => ps.MatchId)
                    .OnDelete(DeleteBehavior.NoAction);

                modelBuilder.Entity<Player>()
                    .HasOne(p => p.Club)
                    .WithMany(c => c.Players)
                    .HasForeignKey(p => p.ClubId);

                modelBuilder.Entity<Club>()
                    .HasOne(c => c.HomeStadium)
                    .WithMany()
                    .HasForeignKey(c => c.HomeStadiumId);

            // The audit log is queried newest-first and filtered by user, so index both.
            modelBuilder.Entity<ActivityLog>()
                .HasIndex(log => log.TimestampUtc);
            modelBuilder.Entity<ActivityLog>()
                .HasIndex(log => log.UserName);
        }
    }
}
