namespace NotionExporter.Applications.Abstractions;

public interface IHandler<in TRequest>
{
    Task ExecuteAsync(TRequest request);
}