namespace FootballClub.Web.Features.Attachments;

/// <summary>
/// Abstracts where attachment files are physically stored. Implemented by
/// <see cref="LocalFileStorage"/> (wwwroot/uploads, for dev/test) and
/// <see cref="BlobFileStorage"/> (Azure Blob Storage, for production).
/// </summary>
public interface IFileStorage
{
    /// <summary>
    /// Persists the file and returns the value to store in Attachment.FilePath
    /// (a relative web path for local storage, or an absolute blob URL for Azure).
    /// </summary>
    Task<string> SaveAsync(Stream content, string entityType, int entityId, string fileName, string contentType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a previously stored file given the value returned by <see cref="SaveAsync"/>.
    /// </summary>
    Task DeleteAsync(string fileLocation, CancellationToken cancellationToken = default);
}
