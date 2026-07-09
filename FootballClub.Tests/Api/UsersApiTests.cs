using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FootballClub.Tests.Api;

public class UsersApiTests
{
    [Fact]
    public async Task GetAll_ReturnsUnauthorized_WhenAnonymous()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory);

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsForbidden_WhenNotAdmin()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsSeededUsers_WhenAdmin()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var users = await client.GetFromJsonAsync<List<UserDto>>("/api/users");

        Assert.NotNull(users);
        Assert.Contains(users!, user => user.Username == "admin" && user.Role == "Admin");
        Assert.Contains(users!, user => user.Username == "user" && user.Role == "User");
    }

    [Fact]
    public async Task Promote_ChangesRoleToAdmin()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var userId = TestDbHelper.UseDb(factory, db => db.Users.First(u => u.UserName == "user").Id);

        var promote = await client.PostAsync($"/api/users/{userId}/promote", null);
        Assert.Equal(HttpStatusCode.NoContent, promote.StatusCode);

        var users = await client.GetFromJsonAsync<List<UserDto>>("/api/users");
        Assert.Equal("Admin", users!.Single(u => u.Username == "user").Role);
    }

    [Fact]
    public async Task Promote_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.PostAsync("/api/users/999999/promote", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesUser()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");
        var userId = TestDbHelper.UseDb(factory, db => db.Users.First(u => u.UserName == "user").Id);

        var delete = await client.DeleteAsync($"/api/users/{userId}");
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);

        var users = await client.GetFromJsonAsync<List<UserDto>>("/api/users");
        Assert.DoesNotContain(users!, user => user.Username == "user");
    }

    [Fact]
    public async Task Delete_OwnAccount_ReturnsBadRequest()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        // The test admin token carries NameIdentifier "1"; deleting that id is a self-delete.
        var response = await client.DeleteAsync("/api/users/1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
