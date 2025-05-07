using System.ComponentModel;
using NotionExporter.Applications.Abstractions;
using NotionExporter.Applications.Requests;
using NotionExporter.Cli.Commands.Base;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NotionExporter.Cli.Commands;

public class DatabaseExportCommand(
    IHandler<DatabaseExportRequest> handler,
    ITokenResolver tokenResolver)
    : ExportCommand<DatabaseExportCommand.Settings, DatabaseExportRequest>(handler, tokenResolver)
{
    protected override DatabaseExportRequest MapRequest(Settings settings, string apiToken)
    {
        return new DatabaseExportRequest
        {
            Token = apiToken,
            Format = settings.Format,
            Id = settings.Id,
            FilterJson = settings.FilterJson,
            FilterFile = settings.FilterFile,
            Output = settings.Output,
            Debug = settings.Debug,
        };
    }

    public class Settings : ExportCommandSettings
    {
        [CommandOption("--id <DATABASE_ID>")]
        [Description("Database ID")]
        public string Id { get; set; } = default!;

        [CommandOption("--filter-json <JSON>")]
        [Description("JSON Notion query (filter + sorts)")]
        public string? FilterJson { get; set; }

        [CommandOption("--filter-file <FILE>")]
        [Description("Path to JSON file contains Notion query (filter + sorts)")]
        public string? FilterFile { get; set; }

        public override ValidationResult Validate()
        {
            return string.IsNullOrWhiteSpace(Id)
                ? ValidationResult.Error("Database ID must be specified")
                : ValidationResult.Success();
        }
    }
}