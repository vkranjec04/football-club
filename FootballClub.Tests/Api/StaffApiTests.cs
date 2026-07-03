using System.Net;
using System.Net.Http.Json;
using FootballClub.Web.Dto;
using Xunit;

namespace FootballClub.Tests.Api;

public class StaffApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Staff");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsStaff_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var staffId = TestDbHelper.UseDb(factory, db => db.StaffMembers.Where(staff => !staff.IsDeleted).Select(staff => staff.Id).First());

        var response = await client.GetAsync($"/api/Staff/{staffId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Staff/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var payload = new StaffCreateDto
        {
            FirstName = "Test",
            LastName = "Coach",
            Nationality = "Test",
            DateOfBirth = new DateTime(1980, 1, 1),
            ContractUntil = new DateTime(2028, 1, 1),
            Role = "Assistant Coach",
            ClubId = clubId
        };

        var response = await client.PostAsJsonAsync("/api/Staff", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsJsonAsync("/api/Staff", new StaffCreateDto());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var staffId = TestDbHelper.UseDb(factory, db => db.StaffMembers.Where(staff => !staff.IsDeleted).Select(staff => staff.Id).First());
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var payload = new StaffUpdateDto
        {
            Id = staffId,
            FirstName = "Updated",
            LastName = "Coach",
            Nationality = "Updated",
            DateOfBirth = new DateTime(1981, 2, 2),
            ContractUntil = new DateTime(2029, 1, 1),
            Role = "Head Coach",
            ClubId = clubId
        };

        var response = await client.PutAsJsonAsync($"/api/Staff/{staffId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var payload = new StaffUpdateDto
        {
            Id = 999999,
            FirstName = "Updated",
            LastName = "Coach",
            Nationality = "Updated",
            DateOfBirth = new DateTime(1981, 2, 2),
            ContractUntil = new DateTime(2029, 1, 1),
            Role = "Head Coach",
            ClubId = clubId
        };

        var response = await client.PutAsJsonAsync("/api/Staff/999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var staffId = TestDbHelper.UseDb(factory, db => db.StaffMembers.Where(staff => !staff.IsDeleted).Select(staff => staff.Id).First());

        var response = await client.DeleteAsync($"/api/Staff/{staffId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/Staff/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
