using FootballClub.Models;

namespace FootballClub.Repositories;

public class ClubMockRepository
{
    public List<Club> GetAll() => MockData.Clubs;
    public Club? GetById(int id) => MockData.Clubs.FirstOrDefault(c => c.Id == id);
    public List<Club> GetByLeague(string leagueName) => MockData.Clubs.Where(c => c.LeagueName == leagueName).OrderBy(c => c.Name).ToList();
    public Club? GetDinamo() => MockData.Clubs.FirstOrDefault(c => c.Name.Contains("Dinamo"));

    public List<LeagueStanding> GetLeagueStandings(string leagueName)
    {
        var leagueClubs = GetByLeague(leagueName);
        var finishedMatches = MockData.Matches
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

public class PlayerMockRepository
{
    public List<Player> GetAll() => MockData.Players;
    public Player? GetById(int id) => MockData.Players.FirstOrDefault(p => p.Id == id);
    public List<Player> GetByClub(int clubId) => MockData.Players.Where(p => p.Club?.Id == clubId).ToList();
    public List<Player> GetByClubOrdered(int clubId) => MockData.Players
        .Where(p => p.Club?.Id == clubId)
        .OrderBy(p => p.Position)
        .ThenBy(p => p.LastName)
        .ToList();
}

public class MatchMockRepository
{
    public List<Match> GetAll() => MockData.Matches;
    public Match? GetById(int id) => MockData.Matches.FirstOrDefault(m => m.Id == id);
    public List<Match> GetUpcoming() => MockData.Matches
        .Where(m => m.Status == Models.Enums.MatchStatus.Scheduled)
        .OrderBy(m => m.Date).ToList();
    public List<Match> GetFinished() => MockData.Matches
        .Where(m => m.Status == Models.Enums.MatchStatus.Finished)
        .OrderByDescending(m => m.Date).ToList();
    
    public List<Match> GetByClub(int clubId) => MockData.Matches
        .Where(m => m.HomeClub.Id == clubId || m.AwayClub.Id == clubId)
        .OrderByDescending(m => m.Date)
        .ToList();
    
    public List<Match> GetUpcomingByClub(int clubId) => MockData.Matches
        .Where(m => (m.HomeClub.Id == clubId || m.AwayClub.Id == clubId) && m.Status == Models.Enums.MatchStatus.Scheduled)
        .OrderBy(m => m.Date)
        .ToList();
    
    public List<Match> GetFinishedByClub(int clubId) => MockData.Matches
        .Where(m => (m.HomeClub.Id == clubId || m.AwayClub.Id == clubId) && m.Status == Models.Enums.MatchStatus.Finished)
        .OrderByDescending(m => m.Date)
        .ToList();
}

public class CoachMockRepository
{
    public List<Coach> GetAll() => MockData.Coaches;
    public Coach? GetById(int id) => MockData.Coaches.FirstOrDefault(c => c.Id == id);
    
    public Coach? GetCurrentCoachByClub(int clubId)
    {
        var club = MockData.Clubs.FirstOrDefault(c => c.Id == clubId);
        return club?.Coach;
    }
    
    public List<Coach> GetCoachesForClub(string clubName)
    {
        // For Dinamo, return coaches (current + past)
        if (clubName.Contains("Dinamo"))
        {
            return MockData.Coaches.Where(c => c.Id == 1 || c.Id == 11 || c.Id == 21).OrderByDescending(c => c.ContractUntil).ToList();
        }
        return new List<Coach>();
    }
}