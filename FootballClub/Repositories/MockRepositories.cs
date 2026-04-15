using FootballClub.Models;

namespace FootballClub.Repositories;

public class ClubMockRepository
{
    public List<Club> GetAll() => MockData.Clubs;
    public Club? GetById(int id) => MockData.Clubs.FirstOrDefault(c => c.Id == id);
}

public class PlayerMockRepository
{
    public List<Player> GetAll() => MockData.Players;
    public Player? GetById(int id) => MockData.Players.FirstOrDefault(p => p.Id == id);
    public List<Player> GetByClub(int clubId) => MockData.Players.Where(p => p.Club?.Id == clubId).ToList();
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
}

public class CoachMockRepository
{
    public List<Coach> GetAll() => MockData.Coaches;
    public Coach? GetById(int id) => MockData.Coaches.FirstOrDefault(c => c.Id == id);
}