namespace FootballClub.Web.Services;

/// <summary>
/// Stores attachments on the local web root (wwwroot/uploads/...). Used for local
/// development and tests where Azure Blob Storage is not configured.
/// </summary>
public class LocalFileStorage : IFileStorage
{
    private readonly IWebHostEnvironment _environment;

    public LocalFileStorage(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(Stream content, string entityType, int entityId, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var relativeDirectory = Path.Combine("uploads", entityType, entityId.ToString());
        var absoluteDirectory = Path.Combine(_environment.WebRootPath, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var uniqueFileName = $"{Guid.NewGuid():N}_{fileName}";
        var absoluteFilePath = Path.Combine(absoluteDirectory, uniqueFileName);
        await using (var stream = File.Create(absoluteFilePath))
        {
            await content.CopyToAsync(stream, cancellationToken);
        }

        return Path.Combine(relativeDirectory, uniqueFileName).Replace('\\', '/');
    }

    public Task DeleteAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(fileLocation))
        {
            var absoluteFilePath = Path.Combine(_environment.WebRootPath, fileLocation.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(absoluteFilePath))
            {
                File.Delete(absoluteFilePath);
            }
        }

        return Task.CompletedTask;
    }
}
