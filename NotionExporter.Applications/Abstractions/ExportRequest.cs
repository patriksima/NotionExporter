using NotionExporter.Shared.Output;

namespace NotionExporter.Applications.Abstractions;

public abstract class ExportRequest
{
    public required string Id { get; set; }
    public required string Token { get; set; }
    public OutputFormat Format { get; set; }
    public string? Output { get; set; }
    public bool? Debug { get; set; }
}