using NotionExporter.Applications.Abstractions;
using NotionExporter.Shared.Output;

namespace NotionExporter.Infrastructure.Output;

public class OutputWriterFactory : IOutputWriterFactory
{
    private readonly Dictionary<OutputFormat, IOutputWriter> _writers;

    public OutputWriterFactory(IEnumerable<IOutputWriter> writers)
    {
        _writers = writers.ToDictionary(w => w.Format);
    }

    public IOutputWriter GetWriter(OutputFormat format)
    {
        return _writers.TryGetValue(format, out var writer)
            ? writer
            : throw new NotSupportedException($"{format} is not supported");
    }
}