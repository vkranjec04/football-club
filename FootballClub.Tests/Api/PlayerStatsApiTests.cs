using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FootballClub.Tests.Api;

public class PlayerStatsApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/PlayerStats");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsStat_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var statId = TestDbHelper.UseDb(factory, db => db.PlayerStats.Select(stat => stat.Id).First());

        var response = await client.GetAsync($"/api/PlayerStats/{statId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/PlayerStats/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());
        var matchId = TestDbHelper.UseDb(factory, db => db.Matches.Select(match => match.Id).First());

        var payload = new PlayerStatCreateDto
        {
            PlayerId = playerId,
            MatchId = matchId,
            Goals = 1,
            Assists = 1,
            MinutesPlayed = 90,
            YellowCards = 0,
            RedCard = false,
            Rating = 8.2
        };

        var response = await client.PostAsJsonAsync("/api/PlayerStats", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsync("/api/PlayerStats", new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var statId = TestDbHelper.UseDb(factory, db => db.PlayerStats.Select(stat => stat.Id).First());
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());
        var matchId = TestDbHelper.UseDb(factory, db => db.Matches.Select(match => match.Id).First());

        var payload = new PlayerStatUpdateDto
        {
            Id = statId,
            PlayerId = playerId,
            MatchId = matchId,
            Goals = 2,
            Assists = 0,
            MinutesPlayed = 80,
            YellowCards = 1,
            RedCard = false,
            Rating = 7.5
        };

        var response = await client.PutAsJsonAsync($"/api/PlayerStats/{statId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());
        var matchId = TestDbHelper.UseDb(factory, db => db.Matches.Select(match => match.Id).First());

        var payload = new PlayerStatUpdateDto
        {
            Id = 999999,
            PlayerId = playerId,
            MatchId = matchId,
            Goals = 2,
            Assists = 0,
            MinutesPlayed = 80,
            YellowCards = 1,
            RedCard = false,
            Rating = 7.5
        };

        var response = await client.PutAsJsonAsync("/api/PlayerStats/999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var statId = TestDbHelper.UseDb(factory, db => db.PlayerStats.Select(stat => stat.Id).First());

        var response = await client.DeleteAsync($"/api/PlayerStats/{statId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/PlayerStats/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
