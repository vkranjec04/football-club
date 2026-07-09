using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using Xunit;

namespace FootballClub.Tests.Mcp;

/// <summary>
/// End-to-end check that the MCP server registered in Program.cs ("/mcp", Streamable HTTP
/// transport) is actually reachable and serves the tools discovered by WithToolsFromAssembly -
/// as opposed to <see cref="McpToolsTests"/>, which calls the tool methods directly.
/// </summary>
public class McpServerIntegrationTests
{
    [Fact]
    public async Task ListTools_IncludesToolsFromEveryFeature()
    {
        await using var factory = new TestWebApplicationFactory(Guid.NewGuid().ToString("N"));
        await using var mcpClient = await ConnectAsync(factory);

        var tools = await mcpClient.ListToolsAsync();
        var names = tools.Select(tool => tool.Name).ToList();

        Assert.Contains("list_clubs", names);
        Assert.Contains("list_players", names);
        Assert.Contains("create_player", names);
        Assert.Contains("get_league_standings", names);
    }

    [Fact]
    public async Task CallTool_ListClubs_ReturnsSeededData()
    {
        await using var factory = new TestWebApplicationFactory(Guid.NewGuid().ToString("N"));
        var clubName = TestDbHelper.UseDb(factory, db => db.Clubs.Select(club => club.Name).First());
        await using var mcpClient = await ConnectAsync(factory);

        var result = await mcpClient.CallToolAsync("list_clubs", new Dictionary<string, object?>());

        Assert.NotEqual(true, result.IsError);
        var text = Assert.IsType<TextContentBlock>(Assert.Single(result.Content)).Text;
        Assert.Contains(clubName, text);
    }

    [Fact]
    public async Task CallTool_GetClub_ReturnsError_WhenMissing()
    {
        await using var factory = new TestWebApplicationFactory(Guid.NewGuid().ToString("N"));
        await using var mcpClient = await ConnectAsync(factory);

        var result = await mcpClient.CallToolAsync("get_club", new Dictionary<string, object?> { ["id"] = 999999 });

        Assert.True(result.IsError);
    }

    private static async Task<McpClient> ConnectAsync(TestWebApplicationFactory factory)
    {
        var httpClient = factory.CreateClient();
        var transport = new HttpClientTransport(
            new HttpClientTransportOptions { Endpoint = new Uri(httpClient.BaseAddress!, "/mcp") },
            httpClient,
            loggerFactory: null!,
            ownsHttpClient: true);

        return await McpClient.CreateAsync(transport);
    }
}
