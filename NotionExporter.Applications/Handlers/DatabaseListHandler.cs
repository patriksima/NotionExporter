using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Requests;

namespace NotionExporter.Applications.Handlers;

public class DatabaseListHandler(INotionApiClient client, IOutputWriterFactory outputWriterFactory) : ExportHandler<DatabaseListRequest>
{
    public override async Task ExecuteAsync(DatabaseListRequest request)
    {
       
        client.SetToken(request.Token);
        
        var jsonDocument = await client.ListDatabasesAsync();
        
        outputWriterFactory.GetWriter(request.Format).Write(jsonDocument, request.Output);
    }
}