using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FootballClub.Tests.Api;

public class AssistantApiTests
{
    [Fact]
    public async Task Chat_ReturnsUnauthorized_WhenNotLoggedIn()
    {
        await using var factory = CreateFactory();
        using var client = factory.CreateClient(); // no bearer token

        var response = await client.PostAsJsonAsync("/api/assistant/chat", new
        {
            messages = new[] { new { role = "user", text = "Who is the most expensive player?" } }
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Chat_ReturnsReply_AndGroundsWithClubData_WhenAuthenticated()
    {
        await using var factory = CreateFactory();
        factory.AiClient.NextChat = new AiChatResult { Success = true, Reply = "FAKE_REPLY" };
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsJsonAsync("/api/assistant/chat", new
        {
            messages = new[] { new { role = "user", text = "Who is the most expensive player?" } }
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("FAKE_REPLY", json.GetProperty("reply").GetString());

        // The endpoint must build and pass a club-data snapshot for grounding.
        Assert.False(string.IsNullOrWhiteSpace(factory.AiClient.LastClubContext));
        Assert.NotNull(factory.AiClient.LastMessages);
        Assert.Equal("user", factory.AiClient.LastMessages![^1].Role);
    }

    [Fact]
    public async Task Chat_ReturnsBadRequest_WhenNoMessages()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsJsonAsync("/api/assistant/chat", new { messages = Array.Empty<object>() });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Chat_ReturnsServiceUnavailable_WhenProviderFails()
    {
        await using var factory = CreateFactory();
        factory.AiClient.NextChat = AiChatResult.Failed("provider down");
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsJsonAsync("/api/assistant/chat", new
        {
            messages = new[] { new { role = "user", text = "hi" } }
        });

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
