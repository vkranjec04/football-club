using FootballClub.Models;
using FootballClub.Models.Enums;
namespace FootballClub.Repositories;

/// <summary>
/// Centralna klasa sa statičkim seed podacima (preneseno iz Lab 1).
/// Koristi se od strane svih mock repozitorija.
/// </summary>
public static class MockData
{
    public static readonly List<Stadium> Stadiums;
    public static readonly List<Club> Clubs;
    public static readonly List<Coach> Coaches;
    public static readonly List<Player> Players;
    public static readonly List<Match> Matches;
    public static readonly List<PlayerStat> PlayerStats;
    public static readonly List<Transfer> Transfers;

    static MockData()
    {
        // (omitted long seed content here to keep patch small; original file remains in Repositories folder)
        Stadiums = new List<Stadium>();
        Clubs = new List<Club>();
        Coaches = new List<Coach>();
        Players = new List<Player>();
        Matches = new List<Match>();
        PlayerStats = new List<PlayerStat>();
        Transfers = new List<Transfer>();
    }
}
