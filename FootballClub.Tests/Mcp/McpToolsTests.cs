using FootballClub.Models.Enums;
using ModelContextProtocol;
using Xunit;

namespace FootballClub.Tests.Mcp;

/// <summary>
/// The MCP tool methods are plain static C# methods (see remarks on McpServerToolAttribute),
/// so they are tested directly against a real, seeded DbContext rather than through the MCP
/// JSON-RPC transport - the transport itself is the SDK's responsibility, not this app's.
/// </summary>
public class McpToolsTests
{
    [Fact]
    public void ListClubs_FiltersByName()
    {
        using var factory = CreateFactory();
        var clubName = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Name).First());

        var results = TestDbHelper.UseDb(factory, db => ClubMcpTools.ListClubs(db, clubName));

        Assert.Contains(results, club => club.Name == clubName);
    }

    [Fact]
    public void GetClub_Throws_WhenMissing()
    {
        using var factory = CreateFactory();

        Assert.Throws<McpException>(() => TestDbHelper.UseDb(factory, db => ClubMcpTools.GetClub(db, 999999)));
    }

    [Fact]
    public void ListPlayers_ExcludesSoftDeleted()
    {
        using var factory = CreateFactory();

        var results = TestDbHelper.UseDb(factory, db => PlayerMcpTools.ListPlayers(db, null, null, null));

        Assert.All(results, player => Assert.False(player.IsDeleted));
    }

    [Fact]
    public void GetPlayer_Throws_WhenMissing()
    {
        using var factory = CreateFactory();

        Assert.Throws<McpException>(() => TestDbHelper.UseDb(factory, db => PlayerMcpTools.GetPlayer(db, 999999)));
    }

    [Fact]
    public void CreatePlayer_ReturnsPlayer_WhenClubExists()
    {
        using var factory = CreateFactory();
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var created = TestDbHelper.UseDb(factory, db => PlayerMcpTools.CreatePlayer(
            db, "Mcp", "Tester", new DateTime(2000, 1, 1), "Test",
            PlayerPosition.Midfielder, 77, 1, new DateTime(2030, 1, 1), clubId));

        Assert.NotEqual(0, created.Id);
        Assert.Equal("Mcp Tester", created.FullName);
        Assert.Equal(clubId, created.ClubId);
    }

    [Fact]
    public void CreatePlayer_Throws_WhenClubMissing()
    {
        using var factory = CreateFactory();

        Assert.Throws<McpException>(() => TestDbHelper.UseDb(factory, db => PlayerMcpTools.CreatePlayer(
            db, "Mcp", "Tester", new DateTime(2000, 1, 1), "Test",
            PlayerPosition.Midfielder, 77, 1, new DateTime(2030, 1, 1), 999999)));
    }

    [Fact]
    public void GetLeagueStandings_OrderedByPointsDescending()
    {
        using var factory = CreateFactory();

        var standings = TestDbHelper.UseDb(factory, db => LeagueMcpTools.GetLeagueStandings(db));

        Assert.Equal(standings.OrderByDescending(standing => standing.Points).Select(standing => standing.Id), standings.Select(standing => standing.Id));
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
