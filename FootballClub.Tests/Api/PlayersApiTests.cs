using System.Net;
using System.Net.Http.Json;
using FootballClub.Models.Enums;
using Xunit;

namespace FootballClub.Tests.Api;

public class PlayersApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Players");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsPlayer_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());

        var response = await client.GetAsync($"/api/Players/{playerId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Players/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var payload = new PlayerCreateDto
        {
            FirstName = "Test",
            LastName = "Player",
            DateOfBirth = new DateTime(2000, 1, 1),
            Nationality = "Test",
            Position = PlayerPosition.Midfielder,
            JerseyNumber = 55,
            MarketValue = 2,
            ContractUntil = new DateTime(2030, 1, 1),
            IsInjured = false,
            ClubId = clubId
        };

        var response = await client.PostAsJsonAsync("/api/Players", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsJsonAsync("/api/Players", new PlayerCreateDto());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var payload = new PlayerUpdateDto
        {
            Id = playerId,
            FirstName = "Updated",
            LastName = "Player",
            DateOfBirth = new DateTime(2001, 2, 2),
            Nationality = "Updated",
            Position = PlayerPosition.Defender,
            JerseyNumber = 77,
            MarketValue = 5,
            ContractUntil = new DateTime(2031, 1, 1),
            IsInjured = true,
            ClubId = clubId
        };

        var response = await client.PutAsJsonAsync($"/api/Players/{playerId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var payload = new PlayerUpdateDto
        {
            Id = 999999,
            FirstName = "Updated",
            LastName = "Player",
            DateOfBirth = new DateTime(2001, 2, 2),
            Nationality = "Updated",
            Position = PlayerPosition.Defender,
            JerseyNumber = 77,
            MarketValue = 5,
            ContractUntil = new DateTime(2031, 1, 1),
            IsInjured = true,
            ClubId = clubId
        };

        var response = await client.PutAsJsonAsync("/api/Players/999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());

        var response = await client.DeleteAsync($"/api/Players/{playerId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/Players/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
