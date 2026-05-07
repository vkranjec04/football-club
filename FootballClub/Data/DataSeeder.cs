using FootballClub.Models;
using FootballClub.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Data;

/// <summary>
/// Helper class to seed the database with MockData on application startup.
/// </summary>
public static class DataSeeder
{
    public static void SeedDatabase(ApplicationDbContext context)
    {
        // Skip if data already exists
        if (context.Stadiums.Any() || context.Clubs.Any())
        {
            return;
        }

        try
        {
            // Add stadiums, clearing IDs so database generates them
            var stadiums = MockData.Stadiums.Select(s => new Stadium 
            { 
                Name = s.Name, 
                City = s.City, 
                Capacity = s.Capacity, 
                YearBuilt = s.YearBuilt 
            }).ToList();
            context.Stadiums.AddRange(stadiums);
            context.SaveChanges();

            // Map stadium IDs from what was saved
            var stadiumMap = new Dictionary<int, int>();
            var savedStadiums = context.Stadiums.ToList();
            foreach (var original in MockData.Stadiums)
            {
                var saved = savedStadiums.FirstOrDefault(s => s.Name == original.Name);
                if (saved != null)
                    stadiumMap[original.Id] = saved.Id;
            }

            // Add clubs
            var clubs = MockData.Clubs.Select(c => new Club 
            { 
                Name = c.Name, 
                City = c.City, 
                FoundedYear = c.FoundedYear, 
                Budget = c.Budget, 
                LeagueName = c.LeagueName,
                HomeStadium = c.HomeStadium != null && stadiumMap.ContainsKey(c.HomeStadium.Id) 
                    ? savedStadiums.First(s => s.Id == stadiumMap[c.HomeStadium.Id]) 
                    : null
            }).ToList();
            context.Clubs.AddRange(clubs);
            context.SaveChanges();

            // Map club IDs
            var clubMap = new Dictionary<int, int>();
            var savedClubs = context.Clubs.ToList();
            foreach (var original in MockData.Clubs)
            {
                var saved = savedClubs.FirstOrDefault(c => c.Name == original.Name);
                if (saved != null)
                    clubMap[original.Id] = saved.Id;
            }

            // Add coaches
            var coaches = MockData.Coaches.Select(c => new Coach 
            { 
                FirstName = c.FirstName, 
                LastName = c.LastName, 
                Nationality = c.Nationality,
                DateOfBirth = c.DateOfBirth,
                ContractUntil = c.ContractUntil,
                Role = c.Role,
                Club = c.Club != null && clubMap.ContainsKey(c.Club.Id) 
                    ? savedClubs.First(cl => cl.Id == clubMap[c.Club.Id]) 
                    : null
            }).ToList();
            context.Coaches.AddRange(coaches);
            context.SaveChanges();

            // Add players
            var players = MockData.Players.Select(p => new Player 
            { 
                FirstName = p.FirstName, 
                LastName = p.LastName, 
                DateOfBirth = p.DateOfBirth,
                Nationality = p.Nationality,
                Position = p.Position,
                JerseyNumber = p.JerseyNumber,
                MarketValue = p.MarketValue,
                ContractUntil = p.ContractUntil,
                IsInjured = p.IsInjured,
                Club = p.Club != null && clubMap.ContainsKey(p.Club.Id) 
                    ? savedClubs.First(cl => cl.Id == clubMap[p.Club.Id]) 
                    : null
            }).ToList();
            context.Players.AddRange(players);
            context.SaveChanges();

            // Map player IDs
            var playerMap = new Dictionary<int, int>();
            var savedPlayers = context.Players.ToList();
            foreach (var original in MockData.Players)
            {
                var saved = savedPlayers.FirstOrDefault(p => p.FirstName == original.FirstName && p.LastName == original.LastName);
                if (saved != null)
                    playerMap[original.Id] = saved.Id;
            }

            // Add matches
            var matches = MockData.Matches.Select(m => new Match 
            { 
                Date = m.Date, 
                HomeClub = m.HomeClub != null && clubMap.ContainsKey(m.HomeClub.Id) 
                    ? savedClubs.First(c => c.Id == clubMap[m.HomeClub.Id]) 
                    : null,
                AwayClub = m.AwayClub != null && clubMap.ContainsKey(m.AwayClub.Id) 
                    ? savedClubs.First(c => c.Id == clubMap[m.AwayClub.Id]) 
                    : null,
                HomeScore = m.HomeScore,
                AwayScore = m.AwayScore,
                Stadium = m.Stadium != null && stadiumMap.ContainsKey(m.Stadium.Id) 
                    ? savedStadiums.First(s => s.Id == stadiumMap[m.Stadium.Id]) 
                    : null,
                Status = m.Status,
                Attendance = m.Attendance,
                Referee = m.Referee,
                Round = m.Round
            }).ToList();
            context.Matches.AddRange(matches);
            context.SaveChanges();

            // Map match IDs
            var matchMap = new Dictionary<int, int>();
            var savedMatches = context.Matches.ToList();
            foreach (var original in MockData.Matches)
            {
                var saved = savedMatches.FirstOrDefault(m => m.Date == original.Date && m.HomeClub != null && m.AwayClub != null);
                if (saved != null)
                    matchMap[original.Id] = saved.Id;
            }

            // Add player stats
            var playerStats = MockData.PlayerStats.Select(ps => new PlayerStat 
            { 
                Player = ps.Player != null && playerMap.ContainsKey(ps.Player.Id) 
                    ? savedPlayers.First(p => p.Id == playerMap[ps.Player.Id]) 
                    : null,
                Match = ps.Match != null && matchMap.ContainsKey(ps.Match.Id) 
                    ? savedMatches.First(m => m.Id == matchMap[ps.Match.Id]) 
                    : null,
                Goals = ps.Goals,
                Assists = ps.Assists,
                MinutesPlayed = ps.MinutesPlayed,
                YellowCards = ps.YellowCards,
                RedCard = ps.RedCard,
                Rating = ps.Rating
            }).ToList();
            context.PlayerStats.AddRange(playerStats);
            context.SaveChanges();

            // Add transfers
            var transfers = MockData.Transfers.Select(t => new Transfer 
            { 
                Player = t.Player != null && playerMap.ContainsKey(t.Player.Id) 
                    ? savedPlayers.First(p => p.Id == playerMap[t.Player.Id]) 
                    : null,
                FromClub = t.FromClub != null && clubMap.ContainsKey(t.FromClub.Id) 
                    ? savedClubs.First(c => c.Id == clubMap[t.FromClub.Id]) 
                    : null,
                ToClub = t.ToClub != null && clubMap.ContainsKey(t.ToClub.Id) 
                    ? savedClubs.First(c => c.Id == clubMap[t.ToClub.Id]) 
                    : null,
                TransferDate = t.TransferDate,
                Fee = t.Fee
            }).ToList();
            context.Transfers.AddRange(transfers);
            context.SaveChanges();

            Console.WriteLine("Database seeded successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error seeding database: {ex.Message}");
            throw;
        }
    }
}
