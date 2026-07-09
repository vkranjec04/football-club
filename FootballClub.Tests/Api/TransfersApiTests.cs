using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FootballClub.Tests.Api;

public class TransfersApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Transfers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsTransfer_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var transferId = TestDbHelper.UseDb(factory, db => db.Transfers.Select(transfer => transfer.Id).First());

        var response = await client.GetAsync($"/api/Transfers/{transferId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Transfers/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());
        var clubIds = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).Take(2).ToList());

        var payload = new TransferCreateDto
        {
            PlayerId = playerId,
            FromClubId = clubIds[0],
            ToClubId = clubIds[1],
            TransferDate = DateTime.UtcNow.Date,
            Fee = 10
        };

        var response = await client.PostAsJsonAsync("/api/Transfers", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsJsonAsync("/api/Transfers", new TransferCreateDto());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var transferId = TestDbHelper.UseDb(factory, db => db.Transfers.Select(transfer => transfer.Id).First());
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());
        var clubIds = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).Take(2).ToList());

        var payload = new TransferUpdateDto
        {
            Id = transferId,
            PlayerId = playerId,
            FromClubId = clubIds[0],
            ToClubId = clubIds[1],
            TransferDate = DateTime.UtcNow.Date.AddDays(-1),
            Fee = 12
        };

        var response = await client.PutAsJsonAsync($"/api/Transfers/{transferId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var playerId = TestDbHelper.UseDb(factory, db => db.Players.Where(player => !player.IsDeleted).Select(player => player.Id).First());
        var clubIds = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).Take(2).ToList());

        var payload = new TransferUpdateDto
        {
            Id = 999999,
            PlayerId = playerId,
            FromClubId = clubIds[0],
            ToClubId = clubIds[1],
            TransferDate = DateTime.UtcNow.Date.AddDays(-1),
            Fee = 12
        };

        var response = await client.PutAsJsonAsync("/api/Transfers/999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var transferId = TestDbHelper.UseDb(factory, db => db.Transfers.Select(transfer => transfer.Id).First());

        var response = await client.DeleteAsync($"/api/Transfers/{transferId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/Transfers/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
