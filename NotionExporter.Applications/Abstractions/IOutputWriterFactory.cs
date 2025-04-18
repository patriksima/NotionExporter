using NotionExporter.Shared.Output;

namespace NotionExporter.Applications.Abstractions;

public interface IOutputWriterFactory
{
    IOutputWriter GetWriter(OutputFormat format);
}