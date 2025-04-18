using System.ComponentModel;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Requests;
using NotionExporter.Cli.Commands.Base;
using Spectre.Console.Cli;

namespace NotionExporter.Cli.Commands;

public class ExportBlocksCommand(
    IHandler<ExportBlocksRequest> handler,
    ITokenResolver tokenResolver)
    : ExportCommand<ExportBlocksCommand.Settings, ExportBlocksRequest>(handler, tokenResolver)
{
    protected override ExportBlocksRequest MapRequest(Settings settings, string apiToken)
    {
        return new ExportBlocksRequest
        {
            Id = settings.Id,
            Token = apiToken,
            Format = settings.Format,
            Output = settings.Output,
            Debug = settings.Debug,
        };
    }

    public class Settings : ExportCommandSettings
    {
        [CommandOption("--id <PAGE_ID>")]
        [Description("Page ID")]
        public string Id { get; set; } = default!;
    }
}