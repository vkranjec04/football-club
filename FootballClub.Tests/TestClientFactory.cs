using System.Net.Http.Headers;

namespace FootballClub.Tests;

internal static class TestClientFactory
{
    internal static HttpClient CreateClient(TestWebApplicationFactory factory, string? role = null)
    {
        var client = factory.CreateClient();
        if (!string.IsNullOrWhiteSpace(role))
        {
            var token = TestJwtTokenGenerator.CreateToken(role);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }
}
