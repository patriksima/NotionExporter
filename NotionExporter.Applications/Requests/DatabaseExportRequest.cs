using NotionExporter.Applications.Abstractions;

namespace NotionExporter.Applications.Requests;

public class DatabaseExportRequest : ExportRequest
{
    public string? FilterJson { get; set; }
    public string? FilterFile { get; set; }
}