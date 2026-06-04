using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FootballClub.Web.Options;
using Microsoft.Extensions.Options;

namespace FootballClub.Web.Services;

/// <summary>
/// Stores attachments in an Azure Blob Storage container. Used in production so files
/// survive App Service restarts/redeploys (the local disk is ephemeral). The container
/// is expected to allow anonymous blob read so Attachment.FilePath can be used directly
/// as a download URL.
/// </summary>
public class BlobFileStorage : IFileStorage
{
    private readonly BlobContainerClient _container;

    public BlobFileStorage(IOptions<StorageOptions> options)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.ConnectionString))
        {
            throw new InvalidOperationException("Storage:ConnectionString is missing.");
        }

        _container = new BlobContainerClient(settings.ConnectionString, settings.ContainerName);
        // Safety net only; the container is normally provisioned ahead of time with
        // public blob access. No-arg create does not change an existing container's ACL.
        _container.CreateIfNotExists();
    }

    public async Task<string> SaveAsync(Stream content, string entityType, int entityId, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var blobName = $"{entityType}/{entityId}/{Guid.NewGuid():N}_{fileName}";
        var blob = _container.GetBlobClient(blobName);
        await blob.UploadAsync(
            content,
            new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } },
            cancellationToken);

        return blob.Uri.ToString();
    }

    public async Task DeleteAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileLocation))
        {
            return;
        }

        var blobName = Uri.TryCreate(fileLocation, UriKind.Absolute, out var uri)
            ? new BlobUriBuilder(uri).BlobName
            : fileLocation;

        await _container.GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}
