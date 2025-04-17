using NotionExporter.Applications.Abstractions;

namespace NotionExporter.Applications.Requests;

public class ExportDatabasesRequest : ExportRequest
{
    public string? FilterJson { get; set; }
    public string? FilterFile { get; set; }
}