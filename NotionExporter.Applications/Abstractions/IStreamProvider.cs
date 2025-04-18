namespace NotionExporter.Applications.Abstractions;

public interface IStreamProvider
{
    Stream GetWriteStream(string? outputPath);
}