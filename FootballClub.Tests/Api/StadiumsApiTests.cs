using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FootballClub.Tests.Api;

public class StadiumsApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Stadiums");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsStadium_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var response = await client.GetAsync($"/api/Stadiums/{stadiumId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Stadiums/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var payload = new StadiumCreateDto
        {
            Name = "Test Stadium",
            City = "Test City",
            Capacity = 10000,
            YearBuilt = 2001
        };

        var response = await client.PostAsJsonAsync("/api/Stadiums", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.PostAsJsonAsync("/api/Stadiums", new StadiumCreateDto());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var payload = new StadiumUpdateDto
        {
            Id = stadiumId,
            Name = "Updated Stadium",
            City = "Updated City",
            Capacity = 20000,
            YearBuilt = 1995
        };

        var response = await client.PutAsJsonAsync($"/api/Stadiums/{stadiumId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var payload = new StadiumUpdateDto
        {
            Id = 999999,
            Name = "Updated Stadium",
            City = "Updated City",
            Capacity = 20000,
            YearBuilt = 1995
        };

        var response = await client.PutAsJsonAsync("/api/Stadiums/999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var stadiumId = TestDbHelper.UseDb(factory, db => db.Stadiums.Select(stadium => stadium.Id).First());

        var response = await client.DeleteAsync($"/api/Stadiums/{stadiumId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/Stadiums/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
