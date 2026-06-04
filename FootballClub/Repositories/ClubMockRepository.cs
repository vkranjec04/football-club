using FootballClub.Data;
using FootballClub.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballClub.Repositories;

public class ClubMockRepository
{
    private readonly ApplicationDbContext _context;

    public ClubMockRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Club> GetAll() => _context.Clubs.ToList();

    public Club? GetById(int id) => _context.Clubs.FirstOrDefault(c => c.Id == id);

    public List<Club> GetByLeague(string leagueName) => _context.Clubs.Where(c => c.LeagueName == leagueName).OrderBy(c => c.Name).ToList();

    public Club? GetDinamo() => _context.Clubs.FirstOrDefault(c => c.Name.Contains("Dinamo"));

    public List<LeagueStanding> GetLeagueStandings(string leagueName)
    {
        var leagueClubs = GetByLeague(leagueName);
        var finishedMatches = _context.Matches
            .Where(m => m.Status == Models.Enums.MatchStatus.Finished)
            .ToList();

        return leagueClubs
            .Select(club =>
            {
                var clubMatches = finishedMatches
                    .Where(m => m.HomeClub.Id == club.Id || m.AwayClub.Id == club.Id)
                    .ToList();

                var wins = clubMatches.Count(m =>
                    (m.HomeClub.Id == club.Id && m.HomeScore > m.AwayScore) ||
                    (m.AwayClub.Id == club.Id && m.AwayScore > m.HomeScore));

                var draws = clubMatches.Count(m => m.HomeScore == m.AwayScore);
                var losses = clubMatches.Count - wins - draws;
                var goalsFor = clubMatches.Sum(m => m.HomeClub.Id == club.Id ? m.HomeScore : m.AwayScore);
                var goalsAgainst = clubMatches.Sum(m => m.HomeClub.Id == club.Id ? m.AwayScore : m.HomeScore);

                return new LeagueStanding
                {
                    Club = club,
                    Played = clubMatches.Count,
                    Wins = wins,
                    Draws = draws,
                    Losses = losses,
                    GoalsFor = goalsFor,
                    GoalsAgainst = goalsAgainst,
                    Points = wins * 3 + draws
                };
            })
            .OrderByDescending(x => x.Points)
            .ThenByDescending(x => x.GoalDiff)
            .ThenByDescending(x => x.GoalsFor)
            .ThenBy(x => x.Club.Name)
            .ToList();
    }
}
