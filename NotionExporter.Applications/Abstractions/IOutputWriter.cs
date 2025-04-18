using System.Text.Json;
using NotionExporter.Shared.Output;

namespace NotionExporter.Applications.Abstractions;

public interface IOutputWriter
{
    OutputFormat Format { get; }
    void Write(JsonDocument document, string? outputPath);
}