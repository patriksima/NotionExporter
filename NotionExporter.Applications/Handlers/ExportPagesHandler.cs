using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Requests;

namespace NotionExporter.Applications.Handlers;

public class ExportPagesHandler(INotionApiClient client, IOutputWriterFactory outputWriterFactory)
    : ExportHandler<ExportPagesRequest>
{
    public override async Task ExecuteAsync(ExportPagesRequest request)
    {
        client.SetToken(request.Token);

        var jsonDocument = await client.RetrievePageAsync(request.Id);

        outputWriterFactory.GetWriter(request.Format).Write(jsonDocument, request.Output);
    }
}