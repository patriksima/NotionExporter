using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Requests;

namespace NotionExporter.Applications.Handlers;

public class ExportDatabasesHandler(INotionApiClient client, IOutputWriterFactory outputWriterFactory) : ExportHandler<ExportDatabasesRequest>
{
    public override async Task ExecuteAsync(ExportDatabasesRequest request)
    {
        string? queryJson = null;
        
        if (!string.IsNullOrWhiteSpace(request.FilterJson))
            queryJson =  request.FilterJson;

        if (!string.IsNullOrWhiteSpace(request.FilterFile))
        {
            if (!File.Exists(request.FilterFile))
                throw new FileNotFoundException($"File '{request.FilterFile}' not found.");

            queryJson = await File.ReadAllTextAsync(request.FilterFile);
        }
        
        client.SetToken(request.Token);
        
        var jsonDocument = await client.QueryDatabaseAsync(request.Id, queryJson);
        
        outputWriterFactory.GetWriter(request.Format).Write(jsonDocument, request.Output);
    }
}