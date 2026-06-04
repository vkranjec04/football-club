using System.ComponentModel.DataAnnotations;

namespace FootballClub.Web.Dto;

public class AttachmentUploadDto : IValidatableObject
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "image/jpeg",
        "image/png",
        "image/gif",
        "image/webp",
        "text/plain"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string EntityType { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int EntityId { get; set; }

    [Required]
    public IFormFile? File { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (File == null)
        {
            yield break;
        }

        if (File.Length > MaxFileSizeBytes)
        {
            yield return new ValidationResult($"File exceeds the maximum size of {MaxFileSizeBytes / 1024 / 1024} MB.", new[] { nameof(File) });
        }

        if (!AllowedContentTypes.Contains(File.ContentType))
        {
            yield return new ValidationResult("Unsupported file type.", new[] { nameof(File) });
        }

        if (SanitizePathSegment(EntityType).Length == 0)
        {
            yield return new ValidationResult("EntityType contains invalid characters.", new[] { nameof(EntityType) });
        }
    }

    private static string SanitizePathSegment(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return new string(value.Where(character => !invalid.Contains(character)).ToArray()).Trim();
    }
}