using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using FootballClub.Web.Dto;
using Xunit;

namespace FootballClub.Tests.Api;

public class AttachmentsApiTests
{
    [Fact]
    public async Task GetAll_ReturnsOk()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var response = await client.GetAsync("/api/Attachments");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsCreated_WhenValidUpload()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var content = CreateMultipartContent("player", "1", "test.txt", "text/plain", "hello");
        var response = await client.PostAsync("/api/Attachments", content);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Post_ReturnsBadRequest_WhenInvalid()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "User");

        var content = new MultipartFormDataContent
        {
            { new StringContent("player"), "EntityType" },
            { new StringContent("1"), "EntityId" }
        };

        var response = await client.PostAsync("/api/Attachments", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenExists()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var content = CreateMultipartContent("player", "1", "delete.txt", "text/plain", "delete");
        var uploadResponse = await client.PostAsync("/api/Attachments", content);
        uploadResponse.EnsureSuccessStatusCode();
        var created = await uploadResponse.Content.ReadFromJsonAsync<AttachmentDto>();
        Assert.NotNull(created);

        var response = await client.DeleteAsync($"/api/Attachments/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMissing()
    {
        await using var factory = CreateFactory();
        using var client = TestClientFactory.CreateClient(factory, "Admin");

        var response = await client.DeleteAsync("/api/Attachments/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private static MultipartFormDataContent CreateMultipartContent(string entityType, string entityId, string fileName, string contentType, string payload)
    {
        var content = new MultipartFormDataContent();
        content.Add(new StringContent(entityType), "EntityType");
        content.Add(new StringContent(entityId), "EntityId");

        var fileBytes = Encoding.UTF8.GetBytes(payload);
        var fileContent = new ByteArrayContent(fileBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        content.Add(fileContent, "File", fileName);

        return content;
    }

    private static TestWebApplicationFactory CreateFactory() => new(Guid.NewGuid().ToString("N"));
}
