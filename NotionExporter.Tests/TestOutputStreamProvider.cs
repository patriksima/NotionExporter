using System.Text;
using NotionExporter.Applications.Abstractions;

namespace NotionExporter.Tests;

public class TestOutputStreamProvider : IStreamProvider
{
    private MemoryStream Stream { get; } = new();

    public Stream GetWriteStream(string? outputPath)
    {
        return Stream;
    }
    
    public string GetCapturedText()
    {
        Stream.Position = 0;
        using var reader = new StreamReader(Stream, Encoding.UTF8, leaveOpen: true);
        return reader.ReadToEnd();
    }
}