using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace FootballClub.Tests.Api;

public class AiExtractionTests
{
    [Fact]
    public async Task ExtractPlayer_MapsFields_AndResolvesClubAndPosition()
    {
        await using var factory = CreateFactory();
        var club = TestDbHelper.UseDb(factory, db => db.Clubs.OrderBy(c => c.Id).Select(c => new { c.Id, c.Name }).First());

        factory.AiClient.NextExtraction = FakeAiClient.Extraction(new
        {
            firstName = "Luka",
            lastName = "Modric",
            nationality = "Croatian",
            position = "Forward",
            jerseyNumber = 10,
            marketValue = 50.0,
            dateOfBirth = "2003-01-01",
            contractUntil = "2027-01-01",
            isInjured = false,
            clubName = club.Name
        });

        using var client = TestClientFactory.CreateClient(factory, "User");
        var response = await client.PostAsync("/ai/extract/player", Form("23-year-old forward Luka Modric"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.GetProperty("success").GetBoolean());

        var data = json.GetProperty("data");
        Assert.Equal("Luka", data.GetProperty("FirstName").GetString());
        Assert.Equal("Modric", data.GetProperty("LastName").GetString());
        Assert.Equal("Forward", data.GetProperty("Position").GetString());
        Assert.Equal(10, data.GetProperty("JerseyNumber").GetInt32());
        Assert.Equal(club.Id, data.GetProperty("ClubId").GetInt32());
        Assert.Equal("2003-01-01T00:00:00", data.GetProperty("DateOfBirth").GetString());

        var dateFields = json.GetProperty("dateFields").EnumerateArray().Select(e => e.GetString()).ToList();
        Assert.Contains("DateOfBirth", dateFields);
        Assert.Contains("ContractUntil", dateFields);
    }

    [Fact]
    public async Task ExtractPlayer_AddsWarning_AndNullClub_WhenClubUnknown()
    {
        await using var factory = CreateFactory();
        factory.AiClient.NextExtraction = FakeAiClient.Extraction(new { firstName = "Test", clubName = "Nonexistent Club XYZ" });

        using var client = TestClientFactory.CreateClient(factory, "User");
        var response = await client.PostAsync("/ai/extract/player", Form("some text"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.GetProperty("success").GetBoolean());
        Assert.Equal(JsonValueKind.Null, json.GetProperty("data").GetProperty("ClubId").ValueKind);
        Assert.NotEmpty(json.GetProperty("warnings").EnumerateArray());
    }

    [Fact]
    public async Task ExtractPlayer_ReturnsFailure_WhenTextEmpty()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.PostAsync("/ai/extract/player", Form(string.Empty));

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.False(json.GetProperty("success").GetBoolean());
    }

    [Fact]
    public async Task ExtractStaff_MapsFields_AndResolvesClub()
    {
        await using var factory = CreateFactory();
        var club = TestDbHelper.UseDb(factory, db => db.Clubs.OrderBy(c => c.Id).Select(c => new { c.Id, c.Name }).First());

        factory.AiClient.NextExtraction = FakeAiClient.Extraction(new
        {
            firstName = "Nenad",
            lastName = "Bjelica",
            nationality = "Croatian",
            dateOfBirth = "1971-01-01",
            contractUntil = "2026-01-01",
            role = "Assistant Coach",
            clubName = club.Name
        });

        using var client = TestClientFactory.CreateClient(factory, "User");
        var response = await client.PostAsync("/ai/extract/staff", Form("assistant coach Bjelica"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var data = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("data");
        Assert.Equal("Nenad", data.GetProperty("FirstName").GetString());
        Assert.Equal("Assistant Coach", data.GetProperty("Role").GetString());
        Assert.Equal(club.Id, data.GetProperty("ClubId").GetInt32());
    }

    [Fact]
    public async Task ExtractTraining_MapsFields_ResolvesIntensityAndLeadStaff()
    {
        await using var factory = CreateFactory();
        var staff = TestDbHelper.UseDb(factory, db => db.StaffMembers
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.Id)
            .Select(s => new { s.Id, Name = s.FirstName + " " + s.LastName })
            .First());

        factory.AiClient.NextExtraction = FakeAiClient.Extraction(new
        {
            title = "Fitness session",
            focusArea = "fitness",
            startTime = "2026-06-10T14:00:00",
            endTime = "2026-06-10T15:30:00",
            location = "Maksimir",
            intensity = "High",
            leadStaffName = staff.Name,
            notes = "stamina"
        });

        using var client = TestClientFactory.CreateClient(factory, "User");
        var response = await client.PostAsync("/ai/extract/training", Form("tomorrow fitness session"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        var data = json.GetProperty("data");
        Assert.Equal("Fitness session", data.GetProperty("Title").GetString());
        Assert.Equal("High", data.GetProperty("Intensity").GetString());
        Assert.Equal(staff.Id, data.GetProperty("LeadStaffId").GetInt32());
        Assert.Equal("2026-06-10T14:00:00", data.GetProperty("StartTime").GetString());

        var dateFields = json.GetProperty("dateFields").EnumerateArray().Select(e => e.GetString()).ToList();
        Assert.Contains("StartTime", dateFields);
        Assert.Contains("EndTime", dateFields);
    }

    private static FormUrlEncodedContent Form(string text)
        => new(new[] { new KeyValuePair<string, string>("text", text) });

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
