namespace FootballClub.Web.Features.Attachments;

public class StorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = "attachments";
}
