using FootballClub.Models;
using FootballClub.Models.Mapping;
using FootballClub.Repositories;
using FootballClub.Web.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballClub.Web.Controllers.Api;

[ApiController]
[Route("api/attachments")]
public class AttachmentsApiController : ApiControllerBase
{
    private readonly AttachmentMockRepository _attachments;
    private readonly IWebHostEnvironment _environment;

    public AttachmentsApiController(AttachmentMockRepository attachments, IWebHostEnvironment environment)
    {
        _attachments = attachments;
        _environment = environment;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll([FromQuery] string? entityType = null, [FromQuery] int? entityId = null)
    {
        var query = _attachments.GetAll().AsEnumerable();
        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(attachment => attachment.EntityType.Equals(entityType.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        if (entityId.HasValue)
        {
            query = query.Where(attachment => attachment.EntityId == entityId.Value);
        }

        var items = query
            .Select(attachment => attachment.ToDto())
            .ToList();

        return Ok(items);
    }

    [Authorize(Roles = "Admin, User")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Upload([FromForm] AttachmentUploadDto dto)
    {
        var safeEntityType = SanitizePathSegment(dto.EntityType.Trim());
        var safeFileName = Path.GetFileName(dto.File!.FileName);
        if (string.IsNullOrWhiteSpace(safeFileName))
        {
            return BadRequest("File name is invalid.");
        }

        var relativeDirectory = Path.Combine("uploads", safeEntityType, dto.EntityId.ToString());
        var absoluteDirectory = Path.Combine(_environment.WebRootPath, relativeDirectory);
        Directory.CreateDirectory(absoluteDirectory);

        var uniqueFileName = $"{Guid.NewGuid():N}_{safeFileName}";
        var absoluteFilePath = Path.Combine(absoluteDirectory, uniqueFileName);
        await using (var stream = System.IO.File.Create(absoluteFilePath))
        {
            await dto.File.CopyToAsync(stream);
        }

        var attachment = new Attachment
        {
            EntityType = dto.EntityType.Trim(),
            EntityId = dto.EntityId,
            FileName = safeFileName,
            FilePath = Path.Combine(relativeDirectory, uniqueFileName).Replace('\\', '/'),
            ContentType = dto.File.ContentType,
            FileSize = dto.File.Length,
            CreatedAt = DateTime.UtcNow
        };

        _attachments.Add(attachment);
        return CreatedAtAction(nameof(GetAll), new { entityType = attachment.EntityType, entityId = attachment.EntityId }, attachment.ToDto());
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var attachment = _attachments.GetById(id);
        if (attachment == null)
        {
            return NotFound();
        }

        var absoluteFilePath = Path.Combine(_environment.WebRootPath, attachment.FilePath.Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(absoluteFilePath))
        {
            System.IO.File.Delete(absoluteFilePath);
        }

        _attachments.Delete(attachment);
        return NoContent();
    }

    private static string SanitizePathSegment(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return new string(value.Where(character => !invalid.Contains(character)).ToArray()).Trim();
    }
}