using System.Text;
using System.Text.Json;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Shared.Output;

namespace NotionExporter.Infrastructure.Output;

public class JsonFileWriter(IStreamProvider streamProvider) : IOutputWriter
{
    public OutputFormat Format => OutputFormat.Json;

    public void Write(JsonDocument document, string? outputPath)
    {
        var stream = streamProvider.GetWriteStream(outputPath);
        var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

        var json = JsonSerializer.Serialize(document, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        writer.WriteLine(json);
    }
}