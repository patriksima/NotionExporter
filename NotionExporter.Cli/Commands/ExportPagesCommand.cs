using System.ComponentModel;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Requests;
using NotionExporter.Cli.Commands.Base;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NotionExporter.Cli.Commands;

public class ExportPagesCommand(
    IHandler<ExportPagesRequest> handler,
    ITokenResolver tokenResolver)
    : ExportCommand<ExportPagesCommand.Settings, ExportPagesRequest>(handler, tokenResolver)
{
    protected override ExportPagesRequest MapRequest(Settings settings, string apiToken)
    {
        return new ExportPagesRequest
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

        public override ValidationResult Validate()
        {
            return string.IsNullOrWhiteSpace(Id)
                ? ValidationResult.Error("Page ID must be specified")
                : ValidationResult.Success();
        }
    }
}