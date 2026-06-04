namespace FootballClub.Web.Options;

public class StorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = "attachments";
}
