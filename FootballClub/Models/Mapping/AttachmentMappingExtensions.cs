using FootballClub.Web.Dto;

namespace FootballClub.Models.Mapping;

public static class AttachmentMappingExtensions
{
    public static AttachmentDto ToDto(this Attachment attachment)
    {
        return new AttachmentDto
        {
            Id = attachment.Id,
            EntityType = attachment.EntityType,
            EntityId = attachment.EntityId,
            FileName = attachment.FileName,
            FilePath = attachment.FilePath,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize,
            CreatedAt = attachment.CreatedAt
        };
    }
}