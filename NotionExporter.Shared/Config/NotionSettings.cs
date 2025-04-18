namespace NotionExporter.Shared.Config;

public class NotionSettings
{
    public string BaseUrl { get; set; } = default!;
    public string? ApiToken { get; set; } = default!;
    public string DatabaseId { get; set; } = default!;
    public string ApiVersion { get; set; } = "2022-06-28";
}