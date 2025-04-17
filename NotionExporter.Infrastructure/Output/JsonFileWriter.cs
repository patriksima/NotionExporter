using System.Text.Json;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Shared.Output;

namespace NotionExporter.Infrastructure.Output;

public class JsonFileWriter : IOutputWriter
{
    public OutputFormat Format => OutputFormat.Json;

    public void Write(JsonDocument document, string? outputPath)
    {
        if (string.IsNullOrWhiteSpace(outputPath))
        {
            var json = JsonSerializer.Serialize(document, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            Console.WriteLine(json);
        }
        else
        {
            using var fs = File.Create(outputPath);
            using var writer = new Utf8JsonWriter(fs, new JsonWriterOptions
            {
                Indented = true
            });

            document.WriteTo(writer);
        }
    }
}