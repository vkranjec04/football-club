using System.Net;
using System.Net.Http.Json;
using FootballClub.Models.Enums;
using FootballClub.Web.Dto;
using Xunit;

namespace FootballClub.Tests.Api;

public class MatchesApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Matches");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsMatch_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var matchId = TestDbHelper.UseDb(factory, db => db.Matches.Select(match => match.Id).First());

        var response = await client.GetAsync($"/api/Matches/{matchId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Matches/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubIds = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).Take(2).ToList());
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var payload = new MatchCreateDto
        {
            Date = DateTime.UtcNow,
            HomeClubId = clubIds[0],
            AwayClubId = clubIds[1],
            HomeScore = 1,
            AwayScore = 0,
            StadiumId = stadiumId,
            Status = MatchStatus.Scheduled,
            Attendance = 1000,
            Referee = "Test Ref",
            Round = "Test Round"
        };

        var response = await client.PostAsJsonAsync("/api/Matches", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsJsonAsync("/api/Matches", new MatchCreateDto());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var matchId = TestDbHelper.UseDb(factory, db => db.Matches.Select(match => match.Id).First());
        var clubIds = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).Take(2).ToList());
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var payload = new MatchUpdateDto
        {
            Id = matchId,
            Date = DateTime.UtcNow.AddDays(1),
            HomeClubId = clubIds[0],
            AwayClubId = clubIds[1],
            HomeScore = 2,
            AwayScore = 2,
            StadiumId = stadiumId,
            Status = MatchStatus.Finished,
            Attendance = 9000,
            Referee = "Updated Ref",
            Round = "Updated Round"
        };

        var response = await client.PutAsJsonAsync($"/api/Matches/{matchId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubIds = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).Take(2).ToList());
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var payload = new MatchUpdateDto
        {
            Id = 999999,
            Date = DateTime.UtcNow.AddDays(1),
            HomeClubId = clubIds[0],
            AwayClubId = clubIds[1],
            HomeScore = 2,
            AwayScore = 2,
            StadiumId = stadiumId,
            Status = MatchStatus.Finished,
            Attendance = 9000,
            Referee = "Updated Ref",
            Round = "Updated Round"
        };

        var response = await client.PutAsJsonAsync("/api/Matches/999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var matchId = TestDbHelper.UseDb(factory, db => db.Matches.Select(match => match.Id).First());

        var response = await client.DeleteAsync($"/api/Matches/{matchId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/Matches/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
