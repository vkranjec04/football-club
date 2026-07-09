using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace FootballClub.Tests.Api;

public class AuthApiTests
{
    [Fact]
    public async Task Register_ReturnsOk_WhenValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory);

        var payload = new RegisterRequestDto
        {
            Username = "newmanager",
            Email = "newmanager@footballclub.local",
            Password = "Passw0rd"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.Token));
        Assert.Equal("User", auth.Role);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenUsernameTaken()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory);

        // "admin" is created by the identity seeder on startup.
        var payload = new RegisterRequestDto
        {
            Username = "admin",
            Email = "different@footballclub.local",
            Password = "Passw0rd"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory);

        // Missing email and a password shorter than the required minimum length.
        var payload = new RegisterRequestDto
        {
            Username = "x",
            Email = "",
            Password = "123"
        };

        var response = await client.PostAsJsonAsync("/api/auth/register", payload);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ReturnsOk_WhenCredentialsValid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory);

        var payload = new LoginRequestDto
        {
            UsernameOrEmail = "admin",
            Password = "Admin123!"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        Assert.NotNull(auth);
        Assert.False(string.IsNullOrWhiteSpace(auth!.Token));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenPasswordWrong()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory);

        var payload = new LoginRequestDto
        {
            UsernameOrEmail = "admin",
            Password = "WrongPassword1!"
        };

        var response = await client.PostAsJsonAsync("/api/auth/login", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
