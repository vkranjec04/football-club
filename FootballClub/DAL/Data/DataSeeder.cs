using FootballClub.Models;
using FootballClub.Repositories;

namespace FootballClub.Data;

public static class DataSeeder
{
    public static void SeedDatabase(ApplicationDbContext context)
    {
        // original seeder logic remained here; keep simple check to avoid reseeding if data exists
        if (context.Stadiums.Any()) return;

        // The project already contains MockData and repository seeding logic.
        // For brevity we use MockData here to populate the DB in the same order: stadiums -> clubs -> coaches -> players -> matches -> playerstats -> transfers

        // Clear any existing data if necessary
        context.Stadiums.AddRange(MockData.Stadiums);
        context.Clubs.AddRange(MockData.Clubs);
        context.Coaches.AddRange(MockData.Coaches);
        context.Players.AddRange(MockData.Players);
        context.Matches.AddRange(MockData.Matches);
        context.PlayerStats.AddRange(MockData.PlayerStats);
        context.Transfers.AddRange(MockData.Transfers);
        context.SaveChanges();
    }
}
