using NotionExporter.Applications.Abstractions;
using NotionExporter.Shared.Output;

namespace NotionExporter.Applications.Requests;

public class DatabaseListRequest
{
    public required string Token { get; set; }
    public OutputFormat Format { get; set; }
    public string? Output { get; set; }
    public bool? Debug { get; set; }
}