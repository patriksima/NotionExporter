namespace NotionExporter.Applications.Abstractions;

public abstract class ExportHandler<TRequest> : IHandler<TRequest>
{
    public abstract Task ExecuteAsync(TRequest request);
}