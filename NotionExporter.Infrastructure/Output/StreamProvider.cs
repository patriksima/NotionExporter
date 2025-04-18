using NotionExporter.Applications.Abstractions;

namespace NotionExporter.Infrastructure.Output;

public class StreamProvider : IStreamProvider
{
    public Stream GetWriteStream(string? outputPath)
    {
        return string.IsNullOrWhiteSpace(outputPath)
            ? Console.OpenStandardOutput()
            : File.Create(outputPath);
    }
}