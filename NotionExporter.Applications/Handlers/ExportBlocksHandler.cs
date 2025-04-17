using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Requests;

namespace NotionExporter.Applications.Handlers;

public class ExportBlocksHandler(INotionApiClient client, IOutputWriterFactory outputWriterFactory)
    : ExportHandler<ExportBlocksRequest>
{
    public override async Task ExecuteAsync(ExportBlocksRequest request)
    {
        client.SetToken(request.Token);
        
        var jsonDocument = await client.RetrieveBlockChildrenAsync(request.Id);

        outputWriterFactory.GetWriter(request.Format).Write(jsonDocument, request.Output);
    }
}