using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Requests;
using NotionExporter.Cli.Commands.Base;

namespace NotionExporter.Cli.Commands;

public class DatabaseListCommand(
    IHandler<DatabaseListRequest> handler,
    ITokenResolver tokenResolver)
    : ExportCommand<DatabaseListCommand.Settings, DatabaseListRequest>(handler, tokenResolver)
{
    protected override DatabaseListRequest MapRequest(Settings settings, string apiToken)
    {
        return new DatabaseListRequest
        {
            Token = apiToken,
            Format = settings.Format,
            Output = settings.Output,
            Debug = settings.Debug,
        };
    }

    public class Settings : ExportCommandSettings
    {
    }
}