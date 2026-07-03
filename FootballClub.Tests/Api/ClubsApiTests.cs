using System.Net;
using System.Net.Http.Json;
using FootballClub.Web.Dto;
using Xunit;

namespace FootballClub.Tests.Api;

public class ClubsApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Clubs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsClub_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var response = await client.GetAsync($"/api/Clubs/{clubId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Clubs/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var payload = new ClubCreateDto
        {
            Name = "Test Club",
            City = "Test City",
            FoundedYear = 2000,
            Budget = 12,
            LeagueName = "Test League",
            HomeStadiumId = stadiumId
        };

        var response = await client.PostAsJsonAsync("/api/Clubs", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.PostAsJsonAsync("/api/Clubs", new ClubCreateDto());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var payload = new ClubUpdateDto
        {
            Id = clubId,
            Name = "Updated Club",
            City = "Updated City",
            FoundedYear = 1999,
            Budget = 99,
            LeagueName = "Updated League",
            HomeStadiumId = stadiumId
        };

        var response = await client.PutAsJsonAsync($"/api/Clubs/{clubId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var payload = new ClubUpdateDto
        {
            Id = 999999,
            Name = "Updated Club",
            City = "Updated City",
            FoundedYear = 1999,
            Budget = 99,
            LeagueName = "Updated League",
            HomeStadiumId = stadiumId
        };

        var response = await client.PutAsJsonAsync("/api/Clubs/999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var response = await client.DeleteAsync($"/api/Clubs/{clubId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/Clubs/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
