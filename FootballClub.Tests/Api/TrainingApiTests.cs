using System.Net;
using System.Net.Http.Json;
using FootballClub.Models.Enums;
using Xunit;

namespace FootballClub.Tests.Api;

public class TrainingApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Training");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsSession_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var sessionId = TestDbHelper.UseDb(factory, db => db.TrainingSessions.Where(session => !session.IsDeleted).Select(session => session.Id).First());

        var response = await client.GetAsync($"/api/Training/{sessionId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Training/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());
        var staffId = TestDbHelper.UseDb(factory, db => db.StaffMembers.Where(staff => !staff.IsDeleted).Select(staff => staff.Id).First());

        var payload = new TrainingSessionCreateDto
        {
            ClubId = clubId,
            Title = "Test Session",
            FocusArea = "Passing",
            StartTime = DateTime.UtcNow.AddDays(1),
            EndTime = DateTime.UtcNow.AddDays(1).AddHours(2),
            Location = "Test Field",
            Intensity = TrainingIntensity.Moderate,
            LeadStaffId = staffId,
            Notes = "Test notes"
        };

        var response = await client.PostAsJsonAsync("/api/Training", payload);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsJsonAsync("/api/Training", new TrainingSessionCreateDto());

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var sessionId = TestDbHelper.UseDb(factory, db => db.TrainingSessions.Where(session => !session.IsDeleted).Select(session => session.Id).First());
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());
        var staffId = TestDbHelper.UseDb(factory, db => db.StaffMembers.Where(staff => !staff.IsDeleted).Select(staff => staff.Id).First());

        var payload = new TrainingSessionUpdateDto
        {
            Id = sessionId,
            ClubId = clubId,
            Title = "Updated Session",
            FocusArea = "Movement",
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(1),
            Location = "Updated Field",
            Intensity = TrainingIntensity.High,
            LeadStaffId = staffId,
            Notes = "Updated notes"
        };

        var response = await client.PutAsJsonAsync($"/api/Training/{sessionId}", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Put_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var payload = new TrainingSessionUpdateDto
        {
            Id = 999999,
            ClubId = clubId,
            Title = "Updated Session",
            FocusArea = "Movement",
            StartTime = DateTime.UtcNow.AddDays(2),
            EndTime = DateTime.UtcNow.AddDays(2).AddHours(1),
            Location = "Updated Field",
            Intensity = TrainingIntensity.High,
            Notes = "Updated notes"
        };

        var response = await client.PutAsJsonAsync("/api/Training/999999", payload);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var sessionId = TestDbHelper.UseDb(factory, db => db.TrainingSessions.Where(session => !session.IsDeleted).Select(session => session.Id).First());

        var response = await client.DeleteAsync($"/api/Training/{sessionId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/Training/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
