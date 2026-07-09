using System.Net;
using System.Net.Http.Json;
using FootballClub.Models.Enums;
using Xunit;

namespace FootballClub.Tests.Api;

public class ActivityLogApiTests
{
    [Fact]
    public async Task GetAll_ReturnsUnauthorized_WhenAnonymous()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory);

        var response = await client.GetAsync("/api/activity-logs");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsForbidden_WhenNotAdmin()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/activity-logs");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsOk_WhenAdmin()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.GetAsync("/api/activity-logs");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PagedResultDto<ActivityLogDto>>();
        Assert.NotNull(page);
    }

    [Fact]
    public async Task FailedLogin_IsAudited()
    {
        await using var factory = CreateFactory();
        using var anonymous = TestClientFactory.CreateClient(factory);
        using var admin = TestClientFactory.CreateClient(factory, "Admin");

        var badLogin = await anonymous.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
        {
            UsernameOrEmail = "admin",
            Password = "WrongPassword1!"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, badLogin.StatusCode);

        var page = await admin.GetFromJsonAsync<PagedResultDto<ActivityLogDto>>("/api/activity-logs?action=LoginFailed");

        Assert.NotNull(page);
        var entry = Assert.Single(page!.Items);
        Assert.Equal("LoginFailed", entry.Action);
        Assert.Equal("Auth", entry.EntityType);
        Assert.Equal("admin", entry.EntityId);
        Assert.False(entry.Success);
    }

    [Fact]
    public async Task StateChangingApiCall_IsAuditedByFilter()
    {
        await using var factory = CreateFactory();
        using var admin = TestClientFactory.CreateClient(factory, "Admin");
        var clubId = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Id).First());

        var created = await admin.PostAsJsonAsync("/api/Players", new PlayerCreateDto
        {
            FirstName = "Audit",
            LastName = "Target",
            DateOfBirth = new DateTime(2000, 1, 1),
            Nationality = "Test",
            Position = PlayerPosition.Midfielder,
            JerseyNumber = 42,
            MarketValue = 1,
            ContractUntil = new DateTime(2030, 1, 1),
            IsInjured = false,
            ClubId = clubId
        });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var page = await admin.GetFromJsonAsync<PagedResultDto<ActivityLogDto>>("/api/activity-logs?action=Create");

        Assert.NotNull(page);
        var entry = page!.Items.FirstOrDefault(log => log.EntityType == "PlayersApi");
        Assert.NotNull(entry);
        Assert.Equal("Create", entry!.Action);
        // The filter resolves the acting user from the bearer token (see TestJwtTokenGenerator).
        Assert.Equal("testuser", entry.UserName);
        Assert.True(entry.Success);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
